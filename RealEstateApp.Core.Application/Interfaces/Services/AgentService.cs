using RealEstateApp.Core.Application.ViewModels.Agents;
using RealEstateApp.Core.Application.ViewModels.AgentProperties;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface AgentService
{
    Task<List<AgenteListViewModel>> GetAgentesActivosAsync(string? nombreFilter);
    Task<AgenteListViewModel?> GetAgenteByIdAsync(string id);
    Task<List<AgentPropertyViewModel>> GetPropiedadesByAgenteAsync(string id);
}
