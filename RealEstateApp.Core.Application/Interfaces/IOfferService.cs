using RealEstateApp.Core.Application.ViewModels.AgentOffers;

namespace RealEstateApp.Core.Application.Interfaces;

public interface IOfferService
{
    Task<List<AgentOfferSummaryViewModel>> GetOfferSummaryByPropertyAsync(int propertyId, string agentId);
    Task<List<AgentOfferDetailViewModel>> GetOffersByClientPropertyAsync(int propertyId, string clientId, string agentId);
    Task AcceptOfferAsync(int offerId, string agentId);
    Task RejectOfferAsync(int offerId, string agentId);
}


