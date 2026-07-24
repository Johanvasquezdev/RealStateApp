using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Domain.Entities;

public class Improvement : AuditableBaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    public ICollection<PropertyImprovement> PropertyImprovements { get; set; } = new List<PropertyImprovement>();
}
