using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RealEstateApp.Core.Application.ViewModels.AgentProfile;

public class AgentProfileSaveViewModel
{
    public string Id { get; set; } = null!;

    [Required(ErrorMessage = "El nombre es requerido")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es requerido")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "El correo es requerido")]
    [EmailAddress(ErrorMessage = "Debe ser un correo válido")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    public string UserName { get; set; } = null!;

    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }

    [DataType(DataType.Password)]
    public string? CurrentPassword { get; set; }

    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }

    [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
    [DataType(DataType.Password)]
    public string? ConfirmNewPassword { get; set; }

    public IFormFile? ProfilePicture { get; set; }
    public string? ExistingProfilePictureUrl { get; set; }
}
