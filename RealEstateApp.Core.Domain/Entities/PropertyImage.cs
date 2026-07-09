namespace RealEstateApp.Core.Domain.Entities;

public class PropertyImage
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = null!;

    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
}
