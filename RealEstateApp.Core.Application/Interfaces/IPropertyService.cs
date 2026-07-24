using RealEstateApp.Core.Application.ViewModels.AgentProperties;

namespace RealEstateApp.Core.Application.Interfaces;

public interface IPropertyService
{
    Task<List<AgentPropertyViewModel>> GetAvailablePropertiesAsync();
    Task<List<AgentPropertyViewModel>> GetPropertiesByAgentAsync(string agentId);
    Task<List<AgentPropertyViewModel>> GetAvailablePropertiesByAgentAsync(string agentId);
    Task<AgentPropertyViewModel?> GetPropertyDetailAsync(int id);
    Task<AgentPropertySaveViewModel> GetPropertyForEditAsync(int id, string agentId);
    Task<int> CreateAsync(AgentPropertySaveViewModel vm, string agentId);
    Task UpdateAsync(AgentPropertySaveViewModel vm, string agentId);
    Task DeleteAsync(int id, string agentId);
    Task<string?> GetFirstImageAsync(int propertyId);
    Task<List<RealEstateApp.Core.Application.DTOs.Properties.PropertyListApiResponse>> GetApiPropertiesAsync();
    Task<RealEstateApp.Core.Application.DTOs.Properties.PropertyApiResponse?> GetApiPropertyByIdAsync(int id);
    Task<RealEstateApp.Core.Application.DTOs.Properties.PropertyApiResponse?> GetApiPropertyByCodeAsync(string code);

}
