using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Messages;

public class MessageSaveViewModel
{
    [Required(ErrorMessage = "El mensaje no puede estar vacío")]
    public string Content { get; set; } = null!;
    
    public int PropertyId { get; set; }
    
    public string? SenderId { get; set; }
}
