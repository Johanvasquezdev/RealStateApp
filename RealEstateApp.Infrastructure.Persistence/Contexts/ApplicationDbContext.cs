using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Identity.Entities;
using System.Reflection;

namespace RealEstateApp.Infrastructure.Persistence.Contexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
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
    public DbSet<SesionPunchCard> SesionesPunchCard { get; set; }
    public DbSet<RegistroPunchCard> RegistrosPunchCard { get; set; }

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
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<ApplicationUser>().ToTable(name: "Users");
        modelBuilder.Entity<IdentityRole>().ToTable(name: "Roles");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable(name: "UserRoles");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable(name: "UserLogins");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable(name: "UserTokens");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable(name: "RoleClaims");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable(name: "UserClaims");

        modelBuilder.Entity<SesionPunchCard>(entity =>
        {
            entity.Property(e => e.NombreArchivo).HasMaxLength(250).IsRequired();
            entity.Property(e => e.UsuarioId).HasMaxLength(450);
            entity.Property(e => e.UsuarioNombre).HasMaxLength(200);
        });

        modelBuilder.Entity<RegistroPunchCard>(entity =>
        {
            entity.HasIndex(e => e.SesionPunchCardId);
            entity.HasIndex(e => new { e.SesionPunchCardId, e.NombreEmpleado, e.Fecha });

            entity.Property(e => e.NombreEmpleado).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Departamento).HasMaxLength(100);
            entity.Property(e => e.HoraEntrada).HasMaxLength(10);
            entity.Property(e => e.HoraSalida).HasMaxLength(10);
            entity.Property(e => e.Observacion).HasMaxLength(500);
            entity.Property(e => e.NombreOriginal).HasMaxLength(200);
            entity.Property(e => e.HorasTrabajadas).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.SesionPunchCard)
                .WithMany(s => s.Registros)
                .HasForeignKey(e => e.SesionPunchCardId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
