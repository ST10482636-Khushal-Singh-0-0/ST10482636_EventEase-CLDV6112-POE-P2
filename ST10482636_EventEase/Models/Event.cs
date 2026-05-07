namespace ST10482636_EventEase.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;    // Added default value
        public string Description { get; set; } = string.Empty;  // Added default value
        public DateTime EventDate { get; set; }
    }
}