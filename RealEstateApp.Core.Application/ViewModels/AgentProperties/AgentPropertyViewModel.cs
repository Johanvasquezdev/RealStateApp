namespace RealEstateApp.Core.Application.ViewModels.AgentProperties;

public class AgentPropertyViewModel
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public double Price { get; set; }
    public string Description { get; set; } = null!;
    public double LandSize { get; set; }
    public int Rooms { get; set; }
    public int Bathrooms { get; set; }
    public string PropertyType { get; set; } = null!;
    public string SaleType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? MainImage { get; set; }
    public List<string> Images { get; set; } = new();
    public List<string> Improvements { get; set; } = new();
}
