using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Property
{
    public class SavePropertyTypeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del tipo de propiedad es requerido.")]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "La descripción es requerida.")]
        public string Description { get; set; } = null!;
    }
}
