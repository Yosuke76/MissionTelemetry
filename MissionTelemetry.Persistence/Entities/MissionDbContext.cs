using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MissionTelemetry.Persistence.Entities;

namespace MissionTelemetry.Persistence
{
    public sealed class MissionDbContext : DbContext
    {
        public MissionDbContext(DbContextOptions<MissionDbContext> options) : base(options) { }

        public DbSet<TelemetrySample> TelemetrySamples => Set<TelemetrySample>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<TelemetrySample>(e =>
            {
                e.ToTable("TelemetrySample");
                e.HasKey(x => x.Id);
                e.Property(x => x.Key)
                .HasMaxLength(128)
                .IsRequired();

                e.HasIndex(x => x.TimeStamp);
                e.HasIndex(x => new { x.Key, x.TimeStamp });
            });
        }
    }
}
