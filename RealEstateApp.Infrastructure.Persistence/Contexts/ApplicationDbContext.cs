using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Entities;
using System.Reflection;

namespace RealEstateApp.Infrastructure.Persistence.Contexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Property> Properties { get; set; }
    public DbSet<PropertyType> PropertyTypes { get; set; }
    public DbSet<SaleType> SaleTypes { get; set; }
    public DbSet<Improvement> Improvements { get; set; }
    public DbSet<PropertyImprovement> PropertyImprovements { get; set; }
    public DbSet<PropertyImage> PropertyImages { get; set; }
    public DbSet<FavoriteProperty> FavoriteProperties { get; set; }
    public DbSet<Offer> Offers { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<PunchCardSession> PunchCardSessions { get; set; }
    public DbSet<PunchCardRecord> PunchCardRecords { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Created = DateTime.UtcNow;
                    entry.Entity.CreatedBy = "DefaultAppUser";
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModified = DateTime.UtcNow;
                    entry.Entity.LastModifiedBy = "DefaultAppUser";
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<PunchCardSession>(entity =>
        {
            entity.Property(e => e.FileName).HasMaxLength(250).IsRequired();
            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.Property(e => e.UserName).HasMaxLength(200);
        });

        modelBuilder.Entity<PunchCardRecord>(entity =>
        {
            entity.HasIndex(e => e.PunchCardSessionId);
            entity.HasIndex(e => new { e.PunchCardSessionId, e.EmployeeName, e.Date });

            entity.Property(e => e.EmployeeName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.ClockInTime).HasMaxLength(10);
            entity.Property(e => e.ClockOutTime).HasMaxLength(10);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.OriginalName).HasMaxLength(200);
            entity.Property(e => e.HoursWorked).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.PunchCardSession)
                .WithMany(s => s.Records)
                .HasForeignKey(e => e.PunchCardSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
