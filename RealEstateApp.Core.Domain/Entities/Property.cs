using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Core.Domain.Enums;

namespace RealEstateApp.Core.Domain.Entities;

public class Property : AuditableBaseEntity
{
    public string Code { get; set; } = null!; // 6 digits code
    public double Price { get; set; }
    public double LandSize { get; set; }
    public int Rooms { get; set; }
    public int Bathrooms { get; set; }
    public string Description { get; set; } = null!;
    public PropertyStatus Status { get; set; } = PropertyStatus.Disponible;

    public string AgentId { get; set; } = null!; // Relates to Identity User

    // Navigation Properties
    public int PropertyTypeId { get; set; }
    public PropertyType PropertyType { get; set; } = null!;

    public int SaleTypeId { get; set; }
    public SaleType SaleType { get; set; } = null!;

    public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
    public ICollection<PropertyImprovement> PropertyImprovements { get; set; } = new List<PropertyImprovement>();
    public ICollection<FavoriteProperty> FavoriteProperties { get; set; } = new List<FavoriteProperty>();
    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
