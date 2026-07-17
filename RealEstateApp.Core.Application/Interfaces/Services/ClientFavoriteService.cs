using RealEstateApp.Core.Application.ViewModels.Properties;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IClientFavoriteService
{
    Task ToggleFavoriteAsync(int propertyId, string clientId);
    Task<List<ClientPropertyViewModel>> GetClientFavoritesAsync(string clientId);
}
