using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Cuenta;

public class LoginViewModel
{
    [Required(ErrorMessage = "Debe ingresar su usuario o correo")]
    public string UsuarioOCorreo { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}
