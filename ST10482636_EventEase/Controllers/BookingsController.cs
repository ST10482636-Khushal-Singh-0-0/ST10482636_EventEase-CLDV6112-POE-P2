using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ST10482636_EventEase.Data;
using ST10482636_EventEase.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ST10482636_EventEase.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ST10482636_EventEaseContext _context;
        private readonly BlobServiceClient _blobServiceClient;

        // Injected both DB context and Azure Blob client structures
        public BookingsController(ST10482636_EventEaseContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var bookings = _context.Booking.Include(b => b.Event).Include(b => b.Venue);
            return View(await bookings.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null) return NotFound();

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName");
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "Name");
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,EventId,VenueId,BookingDate,ImageFile")] Booking booking)
        {
            // Double-booking conflict validation check
            bool isDoubleBooked = await _context.Booking.AnyAsync(b =>
                b.VenueId == booking.VenueId &&
                b.BookingDate.Date == booking.BookingDate.Date);

            if (isDoubleBooked)
            {
                ModelState.AddModelError("BookingDate", "This venue is already reserved for the selected date constraint execution bounds.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle Azure Blob Upload if an image file was supplied
                    if (booking.ImageFile != null)
                    {
                        var containerClient = _blobServiceClient.GetBlobContainerClient("booking-images");
                        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + booking.ImageFile.FileName;
                        var blobClient = containerClient.GetBlobClient(uniqueFileName);

                        using (var stream = booking.ImageFile.OpenReadStream())
                        {
                            await blobClient.UploadAsync(stream, true);
                        }
                        booking.ImageUrl = blobClient.Uri.ToString();
                    }

                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking successfully registered into the deployment ledger.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "A processing exception error occurred while saving the image asset.";
                }
            }
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "Name", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null) return NotFound();

            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "Name", booking.VenueId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,EventId,VenueId,BookingDate,ImageUrl,ImageFile")] Booking booking)
        {
            if (id != booking.BookingId) return NotFound();

            // Overlap verification checking while bypassing its own primary ID tracking entry parameters
            bool isDoubleBooked = await _context.Booking.AnyAsync(b =>
                b.BookingId != booking.BookingId &&
                b.VenueId == booking.VenueId &&
                b.BookingDate.Date == booking.BookingDate.Date);

            if (isDoubleBooked)
            {
                ModelState.AddModelError("BookingDate", "Schedule conflict: Selected space is allocated elsewhere on this targeted calendar matrix window.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (booking.ImageFile != null)
                    {
                        var containerClient = _blobServiceClient.GetBlobContainerClient("booking-images");
                        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + booking.ImageFile.FileName;
                        var blobClient = containerClient.GetBlobClient(uniqueFileName);

                        using (var stream = booking.ImageFile.OpenReadStream())
                        {
                            await blobClient.UploadAsync(stream, true);
                        }
                        booking.ImageUrl = blobClient.Uri.ToString();
                    }

                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking modifications successfully committed.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId)) return NotFound();
                    else throw;
                }
            }
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "Name", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null) return NotFound();

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                _context.Booking.Remove(booking);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Booking cancellation processed.";
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BookingId == id);
        }
    }
}