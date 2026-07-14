using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Messages;

public class MessageSaveViewModel
{
    [Required(ErrorMessage = "El mensaje no puede estar vacío")]
    [StringLength(500, ErrorMessage = "El mensaje no puede exceder los 500 caracteres")]
    [Display(Name = "Mensaje")]
    [DataType(DataType.MultilineText)]
    public string Content { get; set; } = null!;
    
    [Required(ErrorMessage = "La propiedad es requerida")]
    public int PropertyId { get; set; }
    
    public string? SenderId { get; set; }
}
