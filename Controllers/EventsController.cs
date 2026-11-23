using Lab5.Data;
using Lab5.DTOs;
using Lab5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab5.Controllers
{
    // [ApiController] gives automatic 400 responses (ValidationProblemDetails) when a DTO fails its
    // [Required]/[StringLength]/[EmailAddress] rules, and binds request bodies from JSON.
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly EventManagerContext _context;

        public EventsController(EventManagerContext context)
        {
            _context = context;
        }

        /// <summary>Gets all events, each with its list of attendees.</summary>
        /// <response code="200">The list of events (ordered by date).</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents()
        {
            var events = await _context.Events
                .Include(e => e.Attendees)
                .AsNoTracking()
                .OrderBy(e => e.Date)
                .ToListAsync();

            return Ok(events.Select(ToEventDto).ToList());
        }

        /// <summary>Gets one event, with its list of attendees.</summary>
        /// <response code="200">The event.</response>
        /// <response code="404">No event has this id.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventDto>> GetEventById(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Attendees)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null)
            {
                return EventNotFound(id);
            }

            return Ok(ToEventDto(ev));
        }

        /// <summary>Creates a new event.</summary>
        /// <response code="201">The event was created. The Location header is the URL of the new event.</response>
        /// <response code="400">The request body is invalid (missing or too-long fields).</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EventDto>> CreateEvent(CreateEventDto dto)
        {
            var ev = new Event
            {
                Title = dto.Title.Trim(),
                Description = dto.Description.Trim(),
                Date = dto.Date!.Value, // [Required] was already checked by [ApiController]
                Location = dto.Location.Trim()
            };

            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            // 201 Created + Location: /api/events/{id}
            return CreatedAtAction(nameof(GetEventById), new { id = ev.Id }, ToEventDto(ev));
        }

        /// <summary>Updates an existing event.</summary>
        /// <response code="204">The event was updated.</response>
        /// <response code="400">The request body is invalid (missing or too-long fields).</response>
        /// <response code="404">No event has this id.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEvent(int id, UpdateEventDto dto)
        {
            var ev = await _context.Events.FindAsync(id);

            if (ev == null)
            {
                return EventNotFound(id);
            }

            ev.Title = dto.Title.Trim();
            ev.Description = dto.Description.Trim();
            ev.Date = dto.Date!.Value; // [Required] was already checked by [ApiController]
            ev.Location = dto.Location.Trim();

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>Deletes an event. Its attendees are deleted too (cascade delete).</summary>
        /// <response code="204">The event and its attendees were deleted.</response>
        /// <response code="404">No event has this id.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var ev = await _context.Events.FindAsync(id);

            if (ev == null)
            {
                return EventNotFound(id);
            }

            // The Event -> Attendees relationship is configured with OnDelete(DeleteBehavior.Cascade),
            // so the database deletes this event's attendees as well.
            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>Registers an attendee to an event.</summary>
        /// <response code="201">The attendee was registered. The Location header is the URL of the event.</response>
        /// <response code="400">The request body is invalid (missing name, invalid email, ...).</response>
        /// <response code="404">No event has this id.</response>
        /// <response code="409">This email is already registered for the event.</response>
        [HttpPost("{eventId}/attendees")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AttendeeDto>> RegisterAttendee(int eventId, RegisterAttendeeDto dto)
        {
            if (!await _context.Events.AnyAsync(e => e.Id == eventId))
            {
                return EventNotFound(eventId);
            }

            var email = dto.Email.Trim();

            if (await _context.Attendees.AnyAsync(a => a.EventId == eventId && a.Email == email))
            {
                return Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Attendee already registered",
                    detail: $"'{email}' is already registered for event {eventId}.");
            }

            var attendee = new Attendee
            {
                Name = dto.Name.Trim(),
                Email = email,
                EventId = eventId
            };

            _context.Attendees.Add(attendee);
            await _context.SaveChangesAsync();

            // 201 Created + Location: /api/events/{eventId} (the event that now lists this attendee).
            return CreatedAtAction(nameof(GetEventById), new { id = eventId }, ToAttendeeDto(attendee));
        }

        /// <summary>Unregisters (removes) an attendee from an event.</summary>
        /// <response code="204">The attendee was removed from the event.</response>
        /// <response code="404">No event has this id, or the attendee is not registered for this event.</response>
        [HttpDelete("{eventId}/attendees/{attendeeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnregisterAttendee(int eventId, string attendeeId)
        {
            if (!await _context.Events.AnyAsync(e => e.Id == eventId))
            {
                return EventNotFound(eventId);
            }

            var attendee = await _context.Attendees
                .FirstOrDefaultAsync(a => a.Id == attendeeId && a.EventId == eventId);

            if (attendee == null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Attendee not found",
                    detail: $"No attendee with id '{attendeeId}' is registered for event {eventId}.");
            }

            _context.Attendees.Remove(attendee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 404 response with a JSON ProblemDetails body.
        private ObjectResult EventNotFound(int id)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Event not found",
                detail: $"No event with id {id} exists.");
        }

        // ----- Entity -> DTO mapping -----

        private static EventDto ToEventDto(Event ev)
        {
            return new EventDto
            {
                Id = ev.Id,
                Title = ev.Title,
                Description = ev.Description,
                Date = ev.Date,
                Location = ev.Location,
                Attendees = ev.Attendees
                    .OrderBy(a => a.Name)
                    .Select(ToAttendeeDto)
                    .ToList()
            };
        }

        private static AttendeeDto ToAttendeeDto(Attendee attendee)
        {
            return new AttendeeDto
            {
                Id = attendee.Id,
                Name = attendee.Name,
                Email = attendee.Email
            };
        }
    }
}
