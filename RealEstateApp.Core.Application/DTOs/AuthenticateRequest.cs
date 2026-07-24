namespace RealEstateApp.Core.Application.DTOs;

public class AuthenticateRequest
{
    public string UsuarioOCorreo { get; set; } = null!;
    public string Password { get; set; } = null!;
}
