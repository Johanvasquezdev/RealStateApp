namespace RealEstateApp.Core.Application.ViewModels.AgentOffers;

public class AgentOfferManagementViewModel
{
    public int OfferId { get; set; }
    public string ClientId { get; set; } = null!;
    public int PropertyId { get; set; }
}
