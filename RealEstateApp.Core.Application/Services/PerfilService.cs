using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Perfil;

namespace RealEstateApp.Core.Application.Services;

public class PerfilService : IPerfilService
{
    private readonly IUserService _userService;

    public PerfilService(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<PerfilViewModel> GetPerfilAsync(string userId)
    {
        var user = await _userService.FindByIdAsync(userId);
        if (user is null)
            throw new KeyNotFoundException("Usuario no encontrado");

        return new PerfilViewModel
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

    public async Task<bool> UpdatePerfilAsync(string userId, PerfilViewModel vm)
    {
        return await _userService.UpdateUserAsync(userId, vm.FirstName, vm.LastName, vm.Email, vm.PhoneNumber);
    }
}
