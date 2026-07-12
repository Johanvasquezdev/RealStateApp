using RealEstateApp.Core.Application.ViewModels.Properties;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IClientPropertyService
{
    Task<List<ClientPropertyViewModel>> GetPropertiesWithFiltersAsync(ClientFilterPropertyViewModel filters);
    Task<ClientPropertyDetailViewModel?> GetPropertyDetailAsync(int id);
}
