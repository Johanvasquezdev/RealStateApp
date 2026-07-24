using RealEstateApp.Core.Application.ViewModels.Agents;
using RealEstateApp.Core.Application.ViewModels.AgentProperties;

namespace RealEstateApp.Core.Application.Interfaces;

public interface IAgentService
{
    Task<List<AgentListViewModel>> GetActiveAgentsAsync(string? nameFilter);
    Task<List<AgentListViewModel>> GetAllAgentsAsync();
    Task<AgentListViewModel?> GetAgentByIdAsync(string id);
    Task<AgentListViewModel?> GetAgentByIdIncludingInactiveAsync(string id);
    Task<List<AgentPropertyViewModel>> GetPropertiesByAgentAsync(string id);
}


