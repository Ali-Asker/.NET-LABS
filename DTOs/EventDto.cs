namespace Lab5.DTOs
{
    /// <summary>An event as returned by the GET endpoints, including its attendees.</summary>
    public class EventDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string Location { get; set; } = string.Empty;

        public List<AttendeeDto> Attendees { get; set; } = new List<AttendeeDto>();
    }
}
