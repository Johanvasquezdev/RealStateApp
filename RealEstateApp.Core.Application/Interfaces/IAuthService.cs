using RealEstateApp.Core.Application.ViewModels.Accounts;

namespace RealEstateApp.Core.Application.Interfaces;

public interface IAuthService
{
    Task<RegisterResult> RegisterAsync(RegisterViewModel vm);
    Task<LoginResult> LoginAsync(LoginViewModel vm);
    Task LogoutAsync();
}


