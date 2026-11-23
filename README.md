# Lab 5 - Event Manager Web API

A RESTful Web API for the Event Manager domain from Assignments 1 and 2. Clients can create, read, update and
delete **Events**, and register / unregister **Attendees** from an Event.

- ASP.NET Core Web API (controllers), .NET 10
- Entity Framework Core 8 with **SQL Server LocalDB**
- Swagger UI (Swashbuckle.AspNetCore 8)

## Project structure

| Requirement | Where |
| --- | --- |
| NuGet packages (8.x): EF Core, EF Core SqlServer / Design / Tools, Swashbuckle.AspNetCore | `Lab5.csproj` |
| Models (based on Assignment 2) | `Models/Event.cs`, `Models/Attendee.cs` |
| DbContext with `Events` + `Attendees` DbSets; one-to-many + `OnDelete(DeleteBehavior.Cascade)` in `OnModelCreating` | `Data/EventManagerContext.cs` |
| LocalDB connection string | `appsettings.json` (`ConnectionStrings:DefaultConnection`) |
| DbContext + Swagger registration, database creation on startup | `Program.cs` |
| DbInitializer (applies migrations, seeds 3 events x 2 attendees) | `Data/DbInitializer.cs` |
| Migration | `Migrations/` (`InitialCreate`) |
| DTOs | `DTOs/EventDto.cs`, `CreateEventDto.cs`, `UpdateEventDto.cs`, `AttendeeDto.cs`, `RegisterAttendeeDto.cs` |
| Endpoints (`[ApiController]`, `[Route("api/events")]`) | `Controllers/EventsController.cs` |
| Sample requests | `Lab5.http` |

## Endpoints

| Method | Route | Success | Errors |
| --- | --- | --- | --- |
| GET | `/api/events` | 200 + all events (with attendees) | |
| GET | `/api/events/{id}` | 200 + event (with attendees) | 404 event not found |
| POST | `/api/events` | 201 + `Location: /api/events/{id}` + created event | 400 invalid input |
| PUT | `/api/events/{id}` | 204 | 400 invalid input, 404 event not found |
| DELETE | `/api/events/{id}` | 204 (its attendees are deleted too - cascade) | 404 event not found |
| POST | `/api/events/{eventId}/attendees` | 201 + registered attendee (Register) | 400 invalid input, 404 event not found, 409 email already registered for this event |
| DELETE | `/api/events/{eventId}/attendees/{attendeeId}` | 204 (Unregister) | 404 event or attendee not found |

Responses are JSON. Errors use the standard `ProblemDetails` JSON format (`application/problem+json`).

## Run

1. Open `Lab5.slnx` in Visual Studio and press **F5** (or run `dotnet run --launch-profile https`).
2. On startup, `DbInitializer` creates the `EventManagerApiDb` database in `(localdb)\mssqllocaldb`, applies the
   `InitialCreate` migration and seeds 3 events with 2 attendees each.
3. The browser opens **Swagger UI** at `/swagger`, where every endpoint can be tested ("Try it out").

## AI usage

AI was used for this lab. The code was written with the help of **Claude Code** (Anthropic's AI coding assistant),
working from the Lab 5 instructions and my Assignment 2 code:

- `Models/`, `Data/`, `DTOs/`, `Controllers/EventsController.cs`, `Migrations/`
- `Program.cs`, `appsettings.json`, `Properties/launchSettings.json`, `Lab5.csproj`, `Lab5.http`, this README
