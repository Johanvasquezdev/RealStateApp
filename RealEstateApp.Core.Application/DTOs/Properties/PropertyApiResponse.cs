namespace RealEstateApp.Core.Application.DTOs.Properties;

public class PropertyApiResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string PropertyType { get; set; } = null!;
    public string SaleType { get; set; } = null!;
    public decimal Price { get; set; }
    public double LandSize { get; set; }
    public int Rooms { get; set; }
    public int Bathrooms { get; set; }
    public string Description { get; set; } = null!;
    public string Status { get; set; } = null!; // PropertyStatus enum name
    
    public string AgentId { get; set; } = null!;
    public string AgentName { get; set; } = null!;
    
    public List<string> Images { get; set; } = new();
    public List<string> Improvements { get; set; } = new();
}
