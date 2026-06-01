using Microsoft.EntityFrameworkCore;
using ST10482636_EventEase.Models;

namespace ST10482636_EventEase.Data
{
    public class ST10482636_EventEaseContext : DbContext
    {
        public ST10482636_EventEaseContext(DbContextOptions<ST10482636_EventEaseContext> options)
            : base(options)
        {
        }

        public DbSet<Venue> Venue { get; set; } = default!;
        public DbSet<Event> Event { get; set; } = default!;
        public DbSet<Booking> Booking { get; set; } = default!;
        public DbSet<EventType> EventType { get; set; } = default!; // Lookup Table

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Predefine Categories into the Lookup Table
            modelBuilder.Entity<EventType>().HasData(
                new EventType { EventTypeId = 1, TypeName = "Concert/Music Festival" },
                new EventType { EventTypeId = 2, TypeName = "Conference/Corporate" },
                new EventType { EventTypeId = 3, TypeName = "Wedding/Celebration" },
                new EventType { EventTypeId = 4, TypeName = "Exhibition/Expo" },
                new EventType { EventTypeId = 5, TypeName = "Workshop/Seminar" }
            );
        }
    }
}