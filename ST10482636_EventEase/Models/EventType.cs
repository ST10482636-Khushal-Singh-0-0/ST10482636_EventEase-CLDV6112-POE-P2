using System.ComponentModel.DataAnnotations;

namespace ST10482636_EventEase.Models
{
    public class EventType
    {
        public int EventTypeId { get; set; }

        [Required]
        [Display(Name = "Event Category")]
        public string TypeName { get; set; } = string.Empty;
    }
}