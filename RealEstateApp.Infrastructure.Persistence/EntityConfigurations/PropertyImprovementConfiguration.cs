using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations;

public class PropertyImprovementConfiguration : IEntityTypeConfiguration<PropertyImprovement>
{
    public void Configure(EntityTypeBuilder<PropertyImprovement> builder)
    {
        builder.ToTable("PropertyImprovements");
        builder.HasKey(pi => new { pi.PropertyId, pi.ImprovementId });

        builder.HasOne(pi => pi.Property)
            .WithMany(p => p.PropertyImprovements)
            .HasForeignKey(pi => pi.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pi => pi.Improvement)
            .WithMany(i => i.PropertyImprovements)
            .HasForeignKey(pi => pi.ImprovementId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
