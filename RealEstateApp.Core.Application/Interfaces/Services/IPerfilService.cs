using RealEstateApp.Core.Application.ViewModels.Perfil;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IPerfilService
{
    Task<PerfilViewModel> GetPerfilAsync(string userId);
    Task<bool> UpdatePerfilAsync(string userId, PerfilViewModel vm);
}
