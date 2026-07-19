using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Users
{
    public class SaveAdminViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es requerido.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "La cédula es requerida.")]
        public string IdCard { get; set; } = null!;

        [Required(ErrorMessage = "El correo electrónico es requerido.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        public string UserName { get; set; } = null!;

        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
