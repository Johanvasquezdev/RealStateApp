using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RealEstateApp.Core.Application.ViewModels.Cuenta;

public class RegisterViewModel
{
    [Required(ErrorMessage = "El nombre es requerido")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es requerido")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "La cédula es requerida")]
    public string IdCard { get; set; } = null!;

    [Required(ErrorMessage = "El correo es requerido")]
    [EmailAddress(ErrorMessage = "Debe ser un correo válido")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = null!;

    [Required(ErrorMessage = "El tipo de usuario es requerido")]
    public string TipoUsuario { get; set; } = null!;

    public string? PhoneNumber { get; set; }
    public IFormFile? ProfilePicture { get; set; }
}
