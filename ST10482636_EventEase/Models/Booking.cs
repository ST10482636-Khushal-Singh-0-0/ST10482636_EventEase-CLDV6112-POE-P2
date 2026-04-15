namespace ST10482636_EventEase.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int VenueId { get; set; }
        public Venue? Venue { get; set; } // This links to the Venue
        public int EventId { get; set; }
        public Event? Event { get; set; } // This links to the Event
        public DateTime BookingDate { get; set; }
    }
}
