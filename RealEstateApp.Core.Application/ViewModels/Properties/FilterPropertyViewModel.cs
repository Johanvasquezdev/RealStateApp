using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Properties;

public class FilterPropertyViewModel
{
    [Display(Name = "Tipo de Propiedad")]
    public int? PropertyTypeId { get; set; }

    [Display(Name = "Precio Mínimo")]
    [DataType(DataType.Currency)]
    public decimal? MinPrice { get; set; }

    [Display(Name = "Precio Máximo")]
    [DataType(DataType.Currency)]
    public decimal? MaxPrice { get; set; }

    [Display(Name = "Cantidad de Habitaciones")]
    public int? Rooms { get; set; }

    [Display(Name = "Cantidad de Baños")]
    public int? Bathrooms { get; set; }
}
