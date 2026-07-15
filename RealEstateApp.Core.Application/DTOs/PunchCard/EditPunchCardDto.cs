using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.DTOs.PunchCard;

public class EditPunchCardDto
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del empleado es obligatorio.")]
    [StringLength(200)]
    public string NombreEmpleado { get; set; } = string.Empty;

    [Display(Name = "Tiene Licencia")]
    public bool TieneLicencia { get; set; }

    [StringLength(500)]
    public string? Observacion { get; set; }
}
