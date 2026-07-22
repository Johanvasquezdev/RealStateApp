using RealEstateApp.Core.Application.DTOs.Email;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Accounts;

namespace RealEstateApp.Core.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;
    private readonly IFileStorageService _fileStorageService;

    public AuthService(IUserService userService, IEmailService emailService, IFileStorageService fileStorageService)
    {
        _userService = userService;
        _emailService = emailService;
        _fileStorageService = fileStorageService;
    }

    #region RegisterResult
    public async Task<RegisterResult> RegisterAsync(RegisterViewModel vm, string origin)
    {
        if (vm.TipoUsuario != "Cliente" && vm.TipoUsuario != "Agente")
            return new RegisterResult { Exito = false, Mensaje = "Tipo de usuario no válido. Solo se permiten Clientes o Agentes." };

        var existingUser = await _userService.FindByNameAsync(vm.Username);
        if (existingUser is not null)
            return new RegisterResult { Exito = false, Mensaje = "El nombre de usuario ya est en uso." };

        var existingEmail = await _userService.FindByEmailAsync(vm.Email);
        if (existingEmail is not null)
            return new RegisterResult { Exito = false, Mensaje = "El correo electrnico ya est registrado." };

        var normalizedIdCard = NormalizeIdCard(vm.IdCard);
        var existingIdCard = await _userService.FindByIdCardAsync(normalizedIdCard);
        if (existingIdCard is not null)
            return new RegisterResult { Exito = false, Mensaje = "La cédula ya está registrada." };

        string? profilePictureUrl = null;
        if (vm.ProfilePicture is not null)
            profilePictureUrl = _fileStorageService.UploadFile(vm.ProfilePicture, "profile-pictures");

        var (succeeded, error, userId) = await _userService.CreateUserAsync(
            vm.Username, vm.Email, vm.Password, vm.FirstName, vm.LastName,
            vm.PhoneNumber, false, profilePictureUrl, normalizedIdCard);

        if (!succeeded)
            return new RegisterResult { Exito = false, Mensaje = error };

        if (!await _userService.RoleExistsAsync(vm.TipoUsuario))
            await _userService.CreateRoleAsync(vm.TipoUsuario);

        await _userService.AddToRoleAsync(userId!, vm.TipoUsuario);

        var mensaje = "";
        
        if (vm.TipoUsuario == "Cliente")
        {
            var token = await _userService.GenerateEmailConfirmationTokenAsync(userId!);
            token = Uri.EscapeDataString(token);
            var verificationUri = $"{origin}/Account/ConfirmEmail?userId={userId}&token={token}";
            
            var emailRequest = new EmailRequest
            {
                To = vm.Email,
                Subject = "Activa tu cuenta en RealEstateApp",
                Body = $"<h2>¡Bienvenido a RealEstateApp, {vm.FirstName} {vm.LastName}!</h2><p>Su cuenta ha sido registrada correctamente en RealEstateApp.</p><p>Por favor confirma tu correo electrónico y activa tu cuenta haciendo clic en el siguiente enlace: <a href='{verificationUri}'>Activar Cuenta</a></p><p>Si usted no realizó este registro, puede ignorar este mensaje.</p>"
            };
            await _emailService.SendAsync(emailRequest);
            mensaje = "Registro exitoso. Revise su correo para activar su cuenta.";
        }
        else
        {
            mensaje = "Registro exitoso. Un administrador debe activar su cuenta antes de iniciar sesión.";
        }

        return new RegisterResult { Exito = true, Mensaje = mensaje, TipoUsuario = vm.TipoUsuario };
    }
    #endregion

    public async Task<string> ResendActivationEmailAsync(string email, string origin)
    {
        var user = await _userService.FindByEmailAsync(email);
        if (user is null)
            return "No existe una cuenta con ese correo.";

        if (user.IsActive)
            return "Esta cuenta ya se encuentra activa.";

        var roles = await _userService.GetRolesAsync(user.Id);
        if (!roles.Contains("Cliente"))
            return "El reenvío de correo solo aplica para cuentas de Cliente. Las demás cuentas deben ser activadas por un administrador.";

        var token = await _userService.GenerateEmailConfirmationTokenAsync(user.Id);
        token = Uri.EscapeDataString(token);
        var verificationUri = $"{origin}/Account/ConfirmEmail?userId={user.Id}&token={token}";

        var emailRequest = new EmailRequest
        {
            To = user.Email,
            Subject = "Activa tu cuenta en RealEstateApp",
            Body = $"<h2>¡Bienvenido a RealEstateApp, {user.FirstName} {user.LastName}!</h2><p>Su cuenta ha sido registrada correctamente en RealEstateApp.</p><p>Por favor confirma tu correo electrónico y activa tu cuenta haciendo clic en el siguiente enlace: <a href='{verificationUri}'>Activar Cuenta</a></p><p>Si usted no realizó este registro, puede ignorar este mensaje.</p>"
        };
        await _emailService.SendAsync(emailRequest);

        return "El correo de activación ha sido reenviado exitosamente.";
    }

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

    private static string NormalizeIdCard(string? idCard) =>
        new string((idCard ?? string.Empty).Where(char.IsDigit).ToArray());
}



