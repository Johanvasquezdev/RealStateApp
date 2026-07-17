using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Accounts;

namespace RealEstateApp.Core.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;

    public AuthService(IUserService userService)
    {
        _userService = userService;
    }

    #region RegisterResult
    public async Task<RegisterResult> RegisterAsync(RegisterViewModel vm)
    {
        var existingUser = await _userService.FindByNameAsync(vm.Username);
        if (existingUser is not null)
            return new RegisterResult { Exito = false, Mensaje = "El nombre de usuario ya está en uso." };

        var existingEmail = await _userService.FindByEmailAsync(vm.Email);
        if (existingEmail is not null)
            return new RegisterResult { Exito = false, Mensaje = "El correo electrónico ya está registrado." };

        var (succeeded, error, userId) = await _userService.CreateUserAsync(
            vm.Username, vm.Email, vm.Password, vm.FirstName, vm.LastName,
            vm.PhoneNumber, false, null);

        if (!succeeded)
            return new RegisterResult { Exito = false, Mensaje = error };

        if (!await _userService.RoleExistsAsync(vm.TipoUsuario))
            await _userService.CreateRoleAsync(vm.TipoUsuario);

        await _userService.AddToRoleAsync(userId!, vm.TipoUsuario);

        var mensaje = vm.TipoUsuario == "Cliente"
            ? "Registro exitoso. Revise su correo para activar su cuenta."
            : "Registro exitoso. Un administrador debe activar su cuenta antes de iniciar sesión.";

        return new RegisterResult { Exito = true, Mensaje = mensaje, TipoUsuario = vm.TipoUsuario };
    }
    #endregion

    #region LoginResult
    public async Task<LoginResult> LoginAsync(LoginViewModel vm)
    {
        var user = await _userService.FindByEmailAsync(vm.UsuarioOCorreo)
                   ?? await _userService.FindByNameAsync(vm.UsuarioOCorreo);

        if (user is null)
            return new LoginResult { Exito = false, Mensaje = "Los datos de acceso son inválidos." };

        if (!user.IsActive)
            return new LoginResult { Exito = false, Mensaje = "El usuario se encuentra inactivo y no puede iniciar sesión." };

        var roles = await _userService.GetRolesAsync(user.Id);
        if (!roles.Any())
            return new LoginResult { Exito = false, Mensaje = "El usuario no tiene un rol válido asignado. Póngase en contacto con un administrador." };

        if (roles.Contains("Desarrollador"))
            return new LoginResult { Exito = false, Mensaje = "No tiene permisos para acceder a esta aplicación." };

        var signIn = await _userService.PasswordSignInAsync(vm.UsuarioOCorreo, vm.Password);
        if (!signIn.Succeeded)
            return new LoginResult { Exito = false, Mensaje = "Los datos de acceso son inválidos." };

        return new LoginResult { Exito = true, Mensaje = "", Rol = roles.First(), UserId = user.Id };
    } 
    #endregion

    public async Task LogoutAsync()
    {
        await _userService.SignOutAsync();
    }
}



