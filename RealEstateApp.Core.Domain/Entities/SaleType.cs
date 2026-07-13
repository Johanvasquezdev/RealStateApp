using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Domain.Entities;

public class SaleType : AuditableBaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    // Navigation
    public ICollection<Property> Properties { get; set; } = new List<Property>();
}
