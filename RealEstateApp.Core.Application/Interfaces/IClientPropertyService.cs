using RealEstateApp.Core.Application.ViewModels.Properties;

namespace RealEstateApp.Core.Application.Interfaces;

public interface IClientPropertyService
{
    Task<List<ClientPropertyViewModel>> GetPropertiesWithFiltersAsync(ClientFilterPropertyViewModel filters);
    Task<ClientPropertyDetailViewModel?> GetPropertyDetailAsync(int id, string? clientId = null);
}


