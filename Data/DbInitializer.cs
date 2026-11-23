using Lab5.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab5.Data
{
    public static class DbInitializer
    {
        public static void Initialize(EventManagerContext context)
        {
            // Creates the LocalDB database if it doesn't exist and applies any pending
            // migrations (InitialCreate creates the Events and Attendees tables).
            context.Database.Migrate();

            // Only seed an empty database so app restarts don't create duplicates.
            if (context.Events.Any())
            {
                return;
            }

            var events = new Event[]
            {
                new Event
                {
                    Title = "Web API Workshop",
                    Description = "Hands-on workshop: build a RESTful Web API with ASP.NET Core, Entity Framework Core and Swagger.",
                    Date = new DateTime(2027, 1, 20, 10, 0, 0),
                    Location = "Algonquin College - T Building",
                    Attendees = new List<Attendee>
                    {
                        new Attendee { Name = "Alice Smith", Email = "alice@example.com" },
                        new Attendee { Name = "Bob Jones", Email = "bob@example.com" }
                    }
                },
                new Event
                {
                    Title = "Tech Conference 2027",
                    Description = "A full-day conference about cloud, AI and enterprise applications.",
                    Date = new DateTime(2027, 2, 10, 9, 0, 0),
                    Location = "Ottawa Convention Centre",
                    Attendees = new List<Attendee>
                    {
                        new Attendee { Name = "Maria Garcia", Email = "maria@example.com" },
                        new Attendee { Name = "Wei Chen", Email = "wei@example.com" }
                    }
                },
                new Event
                {
                    Title = "EF Core Bootcamp",
                    Description = "Learn DbContext, migrations, relationships and LINQ queries with Entity Framework Core.",
                    Date = new DateTime(2027, 3, 3, 13, 30, 0),
                    Location = "Online",
                    Attendees = new List<Attendee>
                    {
                        new Attendee { Name = "Aisha Khan", Email = "aisha@example.com" },
                        new Attendee { Name = "Liam Brown", Email = "liam@example.com" }
                    }
                }
            };

            context.Events.AddRange(events);
            context.SaveChanges();
        }
    }
}
