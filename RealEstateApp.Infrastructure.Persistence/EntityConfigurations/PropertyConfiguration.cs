using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Properties");

        builder.HasIndex(p => p.Code).IsUnique();
        builder.Property(p => p.Code).IsRequired().HasMaxLength(6);
        builder.Property(p => p.Description).IsRequired();

        builder.HasOne(p => p.PropertyType)
            .WithMany(t => t.Properties)
            .HasForeignKey(p => p.PropertyTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.SaleType)
            .WithMany(t => t.Properties)
            .HasForeignKey(p => p.SaleTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
