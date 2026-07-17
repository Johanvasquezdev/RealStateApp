using RealEstateApp.Core.Application.ViewModels.AgentProfile;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface ProfileService
{
    Task<AgentProfileViewModel> GetProfileAsync(string userId);
    Task<bool> UpdateProfileAsync(string userId, AgentProfileViewModel vm);
}
