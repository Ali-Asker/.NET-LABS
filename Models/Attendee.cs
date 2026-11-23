using System.ComponentModel.DataAnnotations;

namespace Lab5.Models
{
    // Same Attendee as Assignment 2: string Id (a GUID generated when the attendee is created) and an EventId foreign key.
    public class Attendee
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string Email { get; set; } = string.Empty;

        // Foreign key: every attendee belongs to exactly one event.
        public int EventId { get; set; }

        public Event? Event { get; set; }
    }
}
