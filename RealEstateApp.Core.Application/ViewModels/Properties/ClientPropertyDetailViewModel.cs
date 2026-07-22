using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Properties;

public class ClientPropertyDetailViewModel
{
    public int Id { get; set; }

    [Display(Name = "Código")]
    public string Code { get; set; } = null!;

    [Display(Name = "Descripción")]
    public string Description { get; set; } = null!;

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

    public List<string> ImageUrls { get; set; } = new();
    
    [Display(Name = "Mejoras")]
    public List<string> Improvements { get; set; } = new();
    
    public string AgentId { get; set; } = null!;
    
    [Display(Name = "Agente")]
    public string AgentName { get; set; } = null!;
    
    [Display(Name = "Teléfono")]
    [DataType(DataType.PhoneNumber)]
    public string AgentPhone { get; set; } = null!;
    
    [Display(Name = "Correo")]
    [DataType(DataType.EmailAddress)]
    public string AgentEmail { get; set; } = null!;
    
    public string AgentPhotoUrl { get; set; } = null!;

    public bool IsAvailable { get; set; }

    public bool CanMakeOffer { get; set; }
}
