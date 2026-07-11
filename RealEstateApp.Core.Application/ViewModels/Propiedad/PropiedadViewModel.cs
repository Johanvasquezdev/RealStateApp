namespace RealEstateApp.Core.Application.ViewModels.Propiedad;

public class PropiedadViewModel
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
    public string? ImagenPrincipal { get; set; }
    public List<string> Imagenes { get; set; } = new();
    public List<string> Mejoras { get; set; } = new();
}
