using RealEstateApp.Core.Application.ViewModels.AgentProperties;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IPropiedadService
{
    Task<List<AgentPropertyViewModel>> GetPropiedadesDisponiblesAsync();
    Task<List<AgentPropertyViewModel>> GetPropiedadesByAgenteAsync(string agenteId);
    Task<List<AgentPropertyViewModel>> GetPropiedadesDisponiblesByAgenteAsync(string agenteId);
    Task<AgentPropertyViewModel?> GetPropiedadDetailAsync(int id);
    Task<AgentPropertySaveViewModel> GetPropiedadForEditAsync(int id, string agenteId);
    Task<int> CreateAsync(AgentPropertySaveViewModel vm, string agenteId);
    Task UpdateAsync(AgentPropertySaveViewModel vm, string agenteId);
    Task DeleteAsync(int id, string agenteId);
    Task<string?> GetFirstImageAsync(int propertyId);
}
