using RealEstateApp.Core.Application.ViewModels.Offers;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface ClientOfferService
{
    Task<int> CreateOfferAsync(OfferSaveViewModel vm);
    // Para ver el historial de ofertas
    // Usaremos algo como ClientOfferViewModel (podemos reusar o crear luego)
}
