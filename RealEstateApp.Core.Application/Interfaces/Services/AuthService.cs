using RealEstateApp.Core.Application.ViewModels.Accounts;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface AuthService
{
    Task<RegisterResult> RegisterAsync(RegisterViewModel vm);
    Task<LoginResult> LoginAsync(LoginViewModel vm);
    Task LogoutAsync();
}
