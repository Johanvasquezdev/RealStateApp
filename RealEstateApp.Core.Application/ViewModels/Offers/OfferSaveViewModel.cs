using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Offers;

public class OfferSaveViewModel
{
    [Required(ErrorMessage = "El monto de la oferta es requerido")]
    [Range(1, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
    public decimal Amount { get; set; }
    
    public int PropertyId { get; set; }
    
    public string? ClientId { get; set; }
}
