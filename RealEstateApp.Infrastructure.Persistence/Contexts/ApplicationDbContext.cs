using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Interfaces;
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
    }
}
