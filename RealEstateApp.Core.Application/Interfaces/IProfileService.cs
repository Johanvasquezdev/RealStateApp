using RealEstateApp.Core.Application.ViewModels.AgentProfile;

namespace RealEstateApp.Core.Application.Interfaces;

public interface IProfileService
{
    Task<AgentProfileViewModel> GetProfileAsync(string userId);
    Task<bool> UpdateProfileAsync(string userId, AgentProfileViewModel vm);
}


