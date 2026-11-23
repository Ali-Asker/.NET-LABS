using System.ComponentModel.DataAnnotations;

namespace Lab5.Models
{
    // Same Event as Assignment 2, without the Azure Blob Storage BannerUrl (no cloud services in this lab).
    public class Event
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        // One event has many attendees (relationship configured with the Fluent API in EventManagerContext).
        public List<Attendee> Attendees { get; set; } = new List<Attendee>();
    }
}
