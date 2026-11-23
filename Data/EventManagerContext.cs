using Lab5.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab5.Data
{
    public class EventManagerContext : DbContext
    {
        public EventManagerContext(DbContextOptions<EventManagerContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; } = null!;

        public DbSet<Attendee> Attendees { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>(entity =>
            {
                // One-to-many: one Event has many Attendees, each Attendee has one Event.
                // Cascade: deleting an Event also deletes all of its Attendees.
                entity.HasMany(e => e.Attendees)
                      .WithOne(a => a.Event)
                      .HasForeignKey(a => a.EventId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Attendee>(entity =>
            {
                // The string Id holds a GUID ("xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" = 36 characters).
                entity.Property(a => a.Id).HasMaxLength(36);

                // The same email address can only be registered once per event.
                entity.HasIndex(a => new { a.EventId, a.Email }).IsUnique();
            });
        }
    }
}
