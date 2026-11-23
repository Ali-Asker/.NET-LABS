namespace Lab5.DTOs
{
    /// <summary>An attendee as returned inside EventDto (and by the register endpoint).</summary>
    public class AttendeeDto
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}
