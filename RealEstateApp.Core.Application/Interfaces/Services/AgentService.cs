using RealEstateApp.Core.Application.ViewModels.Agents;
using RealEstateApp.Core.Application.ViewModels.AgentProperties;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface AgentService
{
    Task<List<AgentListViewModel>> GetActiveAgentsAsync(string? nameFilter);
    Task<AgentListViewModel?> GetAgentByIdAsync(string id);
    Task<List<AgentPropertyViewModel>> GetPropertiesByAgentAsync(string id);
}
