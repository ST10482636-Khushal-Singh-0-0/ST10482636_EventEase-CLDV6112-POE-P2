using System;
using System.ComponentModel.DataAnnotations;

namespace ST10482636_EventEase.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Please select a Venue.")]
        public int VenueId { get; set; }
        public Venue? Venue { get; set; }

        [Required(ErrorMessage = "Please select an Event.")]
        public int EventId { get; set; }
        public Event? Event { get; set; }

        [Required(ErrorMessage = "Booking Date is required.")]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }
    }
}