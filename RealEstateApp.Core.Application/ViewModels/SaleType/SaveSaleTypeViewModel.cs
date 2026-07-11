using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.SaleType
{
    public class SaveSaleTypeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del tipo de venta es requerido.")]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "La descripción es requerida.")]
        public string Description { get; set; } = null!;
    }
}
