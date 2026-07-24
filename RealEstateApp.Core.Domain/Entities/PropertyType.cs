using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Domain.Entities;

public class PropertyType : AuditableBaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    public ICollection<Property> Properties { get; set; } = new List<Property>();
}
