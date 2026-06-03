using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ST10482636_EventEase.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Please select an event.")]
        [Display(Name = "Event")]
        public int EventId { get; set; }
        public Event? Event { get; set; }

        [Required(ErrorMessage = "Please select a venue.")]
        [Display(Name = "Venue")]
        public int VenueId { get; set; }
        public Venue? Venue { get; set; }

        [Required(ErrorMessage = "Booking date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        // NEW: Image storage properties for Bookings banner attachment
        [Display(Name = "Booking Banner URL")]
        public string? ImageUrl { get; set; }

        [NotMapped]
        [Display(Name = "Upload Booking Banner")]
        public IFormFile? ImageFile { get; set; }
    }
}