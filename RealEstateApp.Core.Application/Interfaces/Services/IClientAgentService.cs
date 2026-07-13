using RealEstateApp.Core.Application.ViewModels.Agents;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IClientAgentService
{
    Task<List<ClientAgentViewModel>> GetAgentsAsync();
    Task<ClientAgentViewModel?> GetAgentDetailAsync(string id);
}
