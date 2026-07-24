using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Offers;

public class OfferSaveViewModel
{
    [Required(ErrorMessage = "El monto de la oferta es requerido")]
    [Range(1, double.MaxValue, ErrorMessage = "El monto de la oferta debe ser mayor a 0")]
    [DataType(DataType.Currency)]
    [Display(Name = "Monto de la Oferta")]
    public decimal Amount { get; set; }
    
    [Required(ErrorMessage = "La propiedad es requerida")]
    public int PropertyId { get; set; }
    
    public string? ClientId { get; set; }
}
