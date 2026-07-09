using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations;

public class ImprovementConfiguration : IEntityTypeConfiguration<Improvement>
{
    public void Configure(EntityTypeBuilder<Improvement> builder)
    {
        builder.ToTable("Improvements");
        builder.Property(i => i.Name).IsRequired().HasMaxLength(100);
        builder.Property(i => i.Description).IsRequired();
    }
}
