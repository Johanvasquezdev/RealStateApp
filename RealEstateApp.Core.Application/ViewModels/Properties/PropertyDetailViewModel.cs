namespace RealEstateApp.Core.Application.ViewModels.Properties;

public class PropertyDetailViewModel
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string PropertyTypeName { get; set; } = null!;
    public string SaleTypeName { get; set; } = null!;
    public decimal Price { get; set; }
    public int Rooms { get; set; }
    public int Bathrooms { get; set; }
    public double LandSize { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<string> Improvements { get; set; } = new();
    
    // Agent info
    public string AgentId { get; set; } = null!;
    public string AgentName { get; set; } = null!;
    public string AgentPhone { get; set; } = null!;
    public string AgentEmail { get; set; } = null!;
    public string AgentPhotoUrl { get; set; } = null!;
}
