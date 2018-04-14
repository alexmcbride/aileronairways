using AileronAirwaysWeb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AileronAirwaysWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Make sure deletions cascade correctly.
            builder.Entity<Timeline>()
                .HasMany(t => t.TimelineEvents)
                .WithOne(e => e.Timeline)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TimelineEvent>()
                .HasMany(e => e.Attachments)
                .WithOne(a => a.TimelineEvent)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<Timeline> Timelines { get; set; }
        public DbSet<TimelineEvent> TimelineEvents { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<ApiEvent> ApiEvents { get; set; }
    }
}
