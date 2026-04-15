using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ST10482636_EventEase.Models;

namespace ST10482636_EventEase.Data
{
    public class ST10482636_EventEaseContext : DbContext
    {
        public ST10482636_EventEaseContext (DbContextOptions<ST10482636_EventEaseContext> options)
            : base(options)
        {
        }

        public DbSet<ST10482636_EventEase.Models.Venue> Venue { get; set; } = default!;
        public DbSet<ST10482636_EventEase.Models.Booking> Booking { get; set; } = default!;
        public DbSet<ST10482636_EventEase.Models.Event> Event { get; set; } = default!;
    }
}
