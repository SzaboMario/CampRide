using CampRide.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampRide.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<CaravanEntity> Caravans => Set<CaravanEntity>();
        public DbSet<BookingEntity> Bookings => Set<BookingEntity>();
        public DbSet<CaravanImageEntity> CaravanImages => Set<CaravanImageEntity>();
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookingEntity>()
                .HasQueryFilter(b => !b.IsDeleted);

            modelBuilder.Entity<CaravanEntity>()
                .HasQueryFilter(c => !c.IsDeleted);

            modelBuilder.Entity<CaravanImageEntity>()
                .HasQueryFilter(i => !i.IsDeleted);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e =>
                    e.Entity is BookingEntity ||
                    e.Entity is CaravanEntity ||
                    e.Entity is CaravanImageEntity);

            foreach(var entry in entries)
            {
                if(entry.State == EntityState.Added)
                {
                    entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                    entry.Property("ChangedAt").CurrentValue = DateTime.UtcNow;
                }

                if(entry.State == EntityState.Modified)
                {
                    entry.Property("ChangedAt").CurrentValue = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }


    }
}
