namespace RealEstateApp.Core.Application.ViewModels.Accounts;

public class LoginResult
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = null!;
    public string? Rol { get; set; }
    public string? UserId { get; set; }
}
