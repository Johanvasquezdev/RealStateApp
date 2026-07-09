using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Identity.Entities;
using System.Reflection;

namespace RealEstateApp.Infrastructure.Persistence.Contexts;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Property> Properties { get; set; }
    public DbSet<PropertyType> PropertyTypes { get; set; }
    public DbSet<SaleType> SaleTypes { get; set; }
    public DbSet<Improvement> Improvements { get; set; }
    public DbSet<PropertyImprovement> PropertyImprovements { get; set; }
    public DbSet<PropertyImage> PropertyImages { get; set; }
    public DbSet<FavoriteProperty> FavoriteProperties { get; set; }
    public DbSet<Offer> Offers { get; set; }
    public DbSet<Message> Messages { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Created = DateTime.UtcNow;
                    entry.Entity.CreatedBy = "DefaultAppUser"; // Pendiente integrar con IHttpContextAccessor
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
        // Must call base first for Identity
        base.OnModelCreating(modelBuilder);

        // Aplica todas las configuraciones de IEntityTypeConfiguration de este ensamblado
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
