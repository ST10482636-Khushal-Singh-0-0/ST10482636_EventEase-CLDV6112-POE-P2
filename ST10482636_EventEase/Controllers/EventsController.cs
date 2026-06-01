using System;
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
    public class EventsController : Controller
    {
        private readonly ST10482636_EventEaseContext _context;
        private readonly BlobServiceClient _blobServiceClient;

        public EventsController(ST10482636_EventEaseContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var events = from e in _context.Event select e;

            if (!string.IsNullOrEmpty(searchString))
            {
                events = events.Where(s => s.EventName.Contains(searchString));
            }

            return View(await events.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event.FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null) return NotFound();

            return View(@event);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,EventName,Description,EventDate,ImageFile")] Event @event)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (@event.ImageFile != null)
                    {
                        var containerClient = _blobServiceClient.GetBlobContainerClient("event-images");
                        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + @event.ImageFile.FileName;
                        var blobClient = containerClient.GetBlobClient(uniqueFileName);

                        using (var stream = @event.ImageFile.OpenReadStream())
                        {
                            await blobClient.UploadAsync(stream, true);
                        }
                        @event.ImageUrl = blobClient.Uri.ToString();
                    }

                    _context.Add(@event);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Event created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "An error occurred while creating the event. Please try again.";
                }
            }
            return View(@event);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event.FindAsync(id);
            if (@event == null) return NotFound();
            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,Description,EventDate,ImageUrl,ImageFile")] Event @event)
        {
            if (id != @event.EventId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (@event.ImageFile != null)
                    {
                        var containerClient = _blobServiceClient.GetBlobContainerClient("event-images");
                        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + @event.ImageFile.FileName;
                        var blobClient = containerClient.GetBlobClient(uniqueFileName);

                        using (var stream = @event.ImageFile.OpenReadStream())
                        {
                            await blobClient.UploadAsync(stream, true);
                        }
                        @event.ImageUrl = blobClient.Uri.ToString();
                    }

                    _context.Update(@event);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Event updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventId)) return NotFound();
                    else throw;
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "An error occurred while updating the event.";
                }
            }
            return View(@event);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event.FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null) return NotFound();

            return View(@event);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (_context.Booking.Any(b => b.EventId == id))
                {
                    TempData["ErrorMessage"] = "Cannot delete this event because it is tied to an active booking.";
                    return RedirectToAction(nameof(Delete), new { id = id });
                }

                var @event = await _context.Event.FindAsync(id);
                if (@event != null)
                {
                    _context.Event.Remove(@event);
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Event deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while trying to delete the event.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.EventId == id);
        }
    }
}