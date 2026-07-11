namespace RealEstateApp.Core.Application.DTOs.Properties;

public class PropertyListApiResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string PropertyType { get; set; } = null!;
    public string SaleType { get; set; } = null!;
    public decimal Price { get; set; }
    public double LandSize { get; set; }
    public int Rooms { get; set; }
    public int Bathrooms { get; set; }
    public string MainImageUrl { get; set; } = null!;
}
