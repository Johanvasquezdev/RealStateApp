using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RealEstateApp.Core.Application.ViewModels.AgentProperties;

public class AgentPropertySaveViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El precio es requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    public double Price { get; set; }

    [Required(ErrorMessage = "La descripción es requerida")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "El tamaño de terreno es requerido")]
    [Range(1, double.MaxValue, ErrorMessage = "El tamaño debe ser mayor a 0")]
    public double LandSize { get; set; }

    [Required(ErrorMessage = "El número de habitaciones es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe tener al menos 1 habitación")]
    public int Rooms { get; set; }

    [Required(ErrorMessage = "El número de baños es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe tener al menos 1 baño")]
    public int Bathrooms { get; set; }

    [Required(ErrorMessage = "El tipo de propiedad es requerido")]
    public int PropertyTypeId { get; set; }

    [Required(ErrorMessage = "El tipo de venta es requerido")]
    public int SaleTypeId { get; set; }

    public List<int> ImprovementIds { get; set; } = new();
    public List<IFormFile>? Images { get; set; }
    public List<string>? ExistingImages { get; set; }
}
