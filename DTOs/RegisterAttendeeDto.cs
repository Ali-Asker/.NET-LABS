using System.ComponentModel.DataAnnotations;

namespace Lab5.DTOs
{
    /// <summary>Body of POST /api/events/{eventId}/attendees.</summary>
    public class RegisterAttendeeDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string Email { get; set; } = string.Empty;
    }
}
