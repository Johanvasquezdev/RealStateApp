namespace RealEstateApp.Core.Domain.Entities;

public class FavoriteProperty
{
    public string ClientId { get; set; } = null!; // Relates to Identity User
    
    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
}
