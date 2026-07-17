namespace RealEstateApp.Core.Application.ViewModels.AgentOffers;

public class AgentOfferDetailViewModel
{
    public int Id { get; set; }
    public string ClientName { get; set; } = null!;
    public double Amount { get; set; }
    public string Status { get; set; } = null!;
    public DateTime Created { get; set; }
}
