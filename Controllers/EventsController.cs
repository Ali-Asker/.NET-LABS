using Assignment_1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_1.Controllers
{
    public class EventsController : Controller
    {
        private static List<Event> Events = new List<Event>
        {
            new Event { Id = 1, Title = "Career Fair", Date = new DateTime(2026, 2, 1), Location = "Gym" },
            new Event { Id = 2, Title = "Tech Talk", Date = new DateTime(2026, 2, 8), Location = "Auditorium" },
            new Event { Id = 3, Title = "Hack Night", Date = new DateTime(2026, 2, 15), Location = "Library" }
        };

        public IActionResult Index()
        {
            ViewData["Title"] = "Event Manager";
            ViewData["Subtitle"] = "Select an event to manage attendees.";
            return View(Events);
        }

        public IActionResult ManageAttendees(int id)
        {
            var selectedEvent = Events.FirstOrDefault(e => e.Id == id);

            if (selectedEvent == null)
            {
                return NotFound();
            }

            ViewData["Title"] = $"Manage Attendees - {selectedEvent.Title}";
            ViewData["EventName"] = selectedEvent.Title;
            return View(selectedEvent);
        }

        [HttpPost]
        public IActionResult SignUp(int id, Attendee attendee)
        {
            var selectedEvent = Events.FirstOrDefault(e => e.Id == id);

            if (selectedEvent == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewData["Title"] = $"Manage Attendees - {selectedEvent.Title}";
                ViewData["EventName"] = selectedEvent.Title;
                return View("ManageAttendees", selectedEvent);
            }

            selectedEvent.Attendees.Add(attendee);
            TempData["Message"] = "Attendee registered!";

            return RedirectToAction(nameof(ManageAttendees), new { id = selectedEvent.Id });
        }
    }
}
