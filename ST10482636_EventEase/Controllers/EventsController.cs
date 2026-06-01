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
    public class EventsController : Controller
    {
        private readonly ST10482636_EventEaseContext _context;
        private readonly BlobServiceClient _blobServiceClient;

        public EventsController(ST10482636_EventEaseContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        // GET: Events with Advanced Filtering
        public async Task<IActionResult> Index(string searchString, int? eventTypeId, DateTime? startDate, DateTime? endDate, int? venueId)
        {
            // Keep filter inputs tracked in the UI view state
            ViewData["CurrentFilter"] = searchString;
            ViewData["SelectedEventType"] = eventTypeId;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");
            ViewData["SelectedVenue"] = venueId;

            // Load values for the Advanced Filter drop-downs
            ViewData["EventTypeId"] = new SelectList(await _context.EventType.ToListAsync(), "EventTypeId", "TypeName", eventTypeId);
            ViewData["VenueId"] = new SelectList(await _context.Venue.ToListAsync(), "VenueId", "Name", venueId);

            var query = _context.Event.Include(e => e.EventType).AsQueryable();

            // Filter 1: Text Search
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.EventName.Contains(searchString) || s.Description.Contains(searchString));
            }

            // Filter 2: Event Type Lookup Category Selection
            if (eventTypeId.HasValue)
            {
                query = query.Where(e => e.EventTypeId == eventTypeId);
            }

            // Filter 3: Date Range Criteria
            if (startDate.HasValue)
            {
                query = query.Where(e => e.EventDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(e => e.EventDate <= endDate.Value);
            }

            // Filter 4: Venue Allocation Availability Checking
            if (venueId.HasValue)
            {
                // Isolate all active event allocations currently mapped to that venue
                var activeBookingEventIds = await _context.Booking
                    .Where(b => b.VenueId == venueId)
                    .Select(b => b.EventId)
                    .ToListAsync();

                query = query.Where(e => activeBookingEventIds.Contains(e.EventId));
            }

            return View(await query.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null) return NotFound();

            return View(@event);
        }

        // GET: Events/Create
        public async Task<IActionResult> Create()
        {
            ViewData["EventTypeId"] = new SelectList(await _context.EventType.ToListAsync(), "EventTypeId", "TypeName");
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,EventName,Description,EventDate,ImageFile,EventTypeId")] Event @event)
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
                    TempData["ErrorMessage"] = "An error occurred during event initialization.";
                }
            }
            ViewData["EventTypeId"] = new SelectList(await _context.EventType.ToListAsync(), "EventTypeId", "TypeName", @event.EventTypeId);
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event.FindAsync(id);
            if (@event == null) return NotFound();

            ViewData["EventTypeId"] = new SelectList(await _context.EventType.ToListAsync(), "EventTypeId", "TypeName", @event.EventTypeId);
            return View(@event);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,Description,EventDate,ImageUrl,ImageFile,EventTypeId")] Event @event)
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
                    TempData["SuccessMessage"] = "Event adjustments successfully updated.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventId)) return NotFound();
                    else throw;
                }
            }
            ViewData["EventTypeId"] = new SelectList(await _context.EventType.ToListAsync(), "EventTypeId", "TypeName", @event.EventTypeId);
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null) return NotFound();

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Booking.Any(b => b.EventId == id))
            {
                TempData["ErrorMessage"] = "Cannot delete this event entity as it is actively assigned to an existing schedule booking entry.";
                return RedirectToAction(nameof(Delete), new { id = id });
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event != null)
            {
                _context.Event.Remove(@event);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Event cleanly removed from system records.";
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.EventId == id);
        }
    }
}