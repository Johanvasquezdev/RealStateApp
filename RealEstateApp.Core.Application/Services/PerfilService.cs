using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.AgentProfile;

namespace RealEstateApp.Core.Application.Services;

public class ProfileService : IProfileService
{
    private readonly IUserService _userService;

    public ProfileService(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<AgentProfileViewModel> GetProfileAsync(string userId)
    {
        var user = await _userService.FindByIdAsync(userId);
        if (user is null)
            throw new KeyNotFoundException("Usuario no encontrado");

        return new AgentProfileViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            UserName = user.UserName,
            PhoneNumber = user.PhoneNumber,
            ExistingProfilePictureUrl = user.ProfilePictureUrl
        };
    }

    public async Task<bool> UpdateProfileAsync(string userId, AgentProfileViewModel vm)
    {
        return await _userService.UpdateUserAsync(userId, vm.FirstName, vm.LastName, vm.Email, vm.PhoneNumber);
    }
}



