using RealEstateApp.Core.Application.ViewModels.Cuenta;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IAuthService
{
    Task<RegisterResult> RegisterAsync(RegisterViewModel vm);
    Task<LoginResult> LoginAsync(LoginViewModel vm);
    Task LogoutAsync();
}
