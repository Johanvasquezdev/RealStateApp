namespace RealEstateApp.Core.Application.ViewModels.Properties;

public class PropertyViewModel
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string PropertyTypeName { get; set; } = null!;
    public string SaleTypeName { get; set; } = null!;
    public decimal Price { get; set; }
    public int Rooms { get; set; }
    public int Bathrooms { get; set; }
    public double LandSize { get; set; }
    public string MainImageUrl { get; set; } = null!;
    public string AgentId { get; set; } = null!;
    public string AgentName { get; set; } = null!;
    public string AgentPhotoUrl { get; set; } = null!;
}
