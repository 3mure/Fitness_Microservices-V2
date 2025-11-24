using Microsoft.EntityFrameworkCore;
using FitnessCalculationService.Domain.Entities;

namespace FitnessCalculationService.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<CalculationHistory> CalculationHistories { get; set; }
        public DbSet<UserCalculationPreferences> UserCalculationPreferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure CalculationHistory
            modelBuilder.Entity<CalculationHistory>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CalculationType);
                entity.HasIndex(e => e.CalculatedAt);
                entity.Property(e => e.InputParameters).HasMaxLength(2000);
                entity.Property(e => e.Result).HasMaxLength(2000);
                entity.Property(e => e.Notes).HasMaxLength(500);
            });

            // Configure UserCalculationPreferences
            modelBuilder.Entity<UserCalculationPreferences>(entity =>
            {
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.Property(e => e.WeightUnit).HasMaxLength(10);
                entity.Property(e => e.HeightUnit).HasMaxLength(10);
                entity.Property(e => e.DistanceUnit).HasMaxLength(10);
                entity.Property(e => e.TemperatureUnit).HasMaxLength(20);
            });

            // Add Global Query Filter for Soft Delete
            modelBuilder.Entity<CalculationHistory>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<UserCalculationPreferences>().HasQueryFilter(e => !e.IsDeleted);
        }

        // Override SaveChanges to handle soft delete and timestamps
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            OnBeforeSaving();
            return base.SaveChanges();
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.CreatedAt = DateTime.UtcNow;
                        entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entity.UpdatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Deleted:
                        // Soft delete
                        entry.State = EntityState.Modified;
                        entity.IsDeleted = true;
                        entity.DeletedAt = DateTime.UtcNow;
                        break;
                }
            }
        }
    }
}

