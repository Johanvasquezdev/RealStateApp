using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Properties;

public class ClientPropertyViewModel
{
    public int Id { get; set; }

    [Display(Name = "Código")]
    public string Code { get; set; } = null!;

    [Display(Name = "Tipo de Propiedad")]
    public string PropertyTypeName { get; set; } = null!;

    [Display(Name = "Tipo de Venta")]
    public string SaleTypeName { get; set; } = null!;

    [Display(Name = "Precio")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    [Display(Name = "Habitaciones")]
    public int Rooms { get; set; }

    [Display(Name = "Baños")]
    public int Bathrooms { get; set; }

    [Display(Name = "Tamaño (m²)")]
    public double LandSize { get; set; }

    public string MainImageUrl { get; set; } = null!;
    
    public string AgentId { get; set; } = null!;
    public string AgentName { get; set; } = null!;
    public string AgentPhotoUrl { get; set; } = null!;
}
