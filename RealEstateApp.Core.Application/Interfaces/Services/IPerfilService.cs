using RealEstateApp.Core.Application.ViewModels.AgentProfile;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IPerfilService
{
    Task<AgentProfileViewModel> GetPerfilAsync(string userId);
    Task<bool> UpdatePerfilAsync(string userId, AgentProfileViewModel vm);
}
