using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10482636_EventEase.Data;
using ST10482636_EventEase.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ST10482636_EventEase.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ST10482636_EventEaseContext _context;
        private readonly BlobServiceClient _blobServiceClient;

        public VenuesController(ST10482636_EventEaseContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        // GET: Venues
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var venues = from v in _context.Venue select v;

            if (!string.IsNullOrEmpty(searchString))
            {
                venues = venues.Where(s => s.Name.Contains(searchString) || s.Location.Contains(searchString));
            }

            return View(await venues.ToListAsync());
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venue.FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueId,Name,Location,Capacity,ImageFile")] Venue venue)
        {
            if (ModelState.IsValid)
            {
                if (venue.ImageFile != null)
                {
                    var containerClient = _blobServiceClient.GetBlobContainerClient("venue-images");
                    await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + venue.ImageFile.FileName;
                    var blobClient = containerClient.GetBlobClient(uniqueFileName);

                    using (var stream = venue.ImageFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, true);
                    }
                    venue.ImageUrl = blobClient.Uri.ToString();
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venue.FindAsync(id);
            if (venue == null) return NotFound();
            return View(venue);
        }

        // POST: Venues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,Name,Location,Capacity,ImageUrl,ImageFile")] Venue venue)
        {
            if (id != venue.VenueId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (venue.ImageFile != null)
                    {
                        var containerClient = _blobServiceClient.GetBlobContainerClient("venue-images");
                        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + venue.ImageFile.FileName;
                        var blobClient = containerClient.GetBlobClient(uniqueFileName);

                        using (var stream = venue.ImageFile.OpenReadStream())
                        {
                            await blobClient.UploadAsync(stream, true);
                        }
                        venue.ImageUrl = blobClient.Uri.ToString();
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venue.FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Booking.Any(b => b.VenueId == id))
            {
                TempData["ErrorMessage"] = "Cannot delete this venue because it has active bookings.";
                return RedirectToAction(nameof(Delete), new { id = id });
            }

            var venue = await _context.Venue.FindAsync(id);
            if (venue != null)
            {
                _context.Venue.Remove(venue);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venue.Any(e => e.VenueId == id);
        }
    }
}