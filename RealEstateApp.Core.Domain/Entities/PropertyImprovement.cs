namespace RealEstateApp.Core.Domain.Entities;

public class PropertyImprovement
{
    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    public int ImprovementId { get; set; }
    public Improvement Improvement { get; set; } = null!;
}
