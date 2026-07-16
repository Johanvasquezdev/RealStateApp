using RealEstateApp.Core.Application.ViewModels.Offers;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface ClientOfferService
{
    Task<int> CreateOfferAsync(OfferSaveViewModel vm);
    Task<List<ClientOfferListViewModel>> GetClientOffersAsync(string clientId);
    Task WithdrawOfferAsync(int offerId, string clientId);
}
