using System.ComponentModel.DataAnnotations;

namespace Lab5.DTOs
{
    /// <summary>Body of PUT /api/events/{id}. Same fields as CreateEventDto.</summary>
    public class UpdateEventDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        // Nullable so that a missing date fails [Required] (400) instead of silently becoming 0001-01-01.
        [Required]
        public DateTime? Date { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;
    }
}
