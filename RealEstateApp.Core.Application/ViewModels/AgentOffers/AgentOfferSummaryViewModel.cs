namespace RealEstateApp.Core.Application.ViewModels.AgentOffers;

public class AgentOfferSummaryViewModel
{
    public string ClientId { get; set; } = null!;
    public string ClientName { get; set; } = null!;
    public int OfferCount { get; set; }
    public double LatestOfferAmount { get; set; }
    public string Status { get; set; } = null!;
}
