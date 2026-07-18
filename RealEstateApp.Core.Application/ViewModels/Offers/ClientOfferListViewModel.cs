using RealEstateApp.Core.Domain.Enums;

namespace RealEstateApp.Core.Application.ViewModels.Offers;

public class ClientOfferListViewModel
{
    public int OfferId { get; set; }
    public int PropertyId { get; set; }
    public string PropertyName { get; set; } = null!;
    public string PropertyCode { get; set; } = null!;
    public string PropertyImageUrl { get; set; } = null!;
    public decimal Amount { get; set; }
    public OfferStatus Status { get; set; } // "Pendiente", "Aceptada", "Rechazada"
    public DateTime CreatedAt { get; set; }
    public string AgentName { get; set; } = null!;
    public string AgentEmail { get; set; } = null!;
    public string? AgentPhotoUrl { get; set; }
}
