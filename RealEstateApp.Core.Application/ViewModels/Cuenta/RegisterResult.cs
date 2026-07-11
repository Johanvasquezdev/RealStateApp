namespace RealEstateApp.Core.Application.ViewModels.Cuenta;

public class RegisterResult
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = null!;
    public string? TipoUsuario { get; set; }
}
