using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations;

public class SaleTypeConfiguration : IEntityTypeConfiguration<SaleType>
{
    public void Configure(EntityTypeBuilder<SaleType> builder)
    {
        builder.ToTable("SaleTypes");
        builder.Property(st => st.Name).IsRequired().HasMaxLength(100);
        builder.Property(st => st.Description).IsRequired();
    }
}
