using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Properties;

public class ClientFilterPropertyViewModel : IValidatableObject
{
    [Display(Name = "Tipo de Propiedad")]
    public int? PropertyTypeId { get; set; }

    [Display(Name = "Precio Mínimo")]
    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue, ErrorMessage = "El precio mínimo no puede ser negativo.")]
    public decimal? MinPrice { get; set; }

    [Display(Name = "Precio Máximo")]
    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue, ErrorMessage = "El precio máximo no puede ser negativo.")]
    public decimal? MaxPrice { get; set; }

    [Display(Name = "Cantidad de Habitaciones")]
    [Range(0, int.MaxValue, ErrorMessage = "La cantidad de habitaciones no puede ser menor que cero.")]
    public int? Rooms { get; set; }

    [Display(Name = "Cantidad de Baños")]
    [Range(0, int.MaxValue, ErrorMessage = "La cantidad de baños no puede ser menor que cero.")]
    public int? Bathrooms { get; set; }

    [Display(Name = "Código")]
    public string? Code { get; set; }

    public string? AgentId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinPrice.HasValue && MaxPrice.HasValue && MinPrice.Value > MaxPrice.Value)
        {
            yield return new ValidationResult(
                "El precio mínimo no puede ser mayor que el precio máximo.",
                new[] { nameof(MinPrice), nameof(MaxPrice) }
            );
        }
    }
}
