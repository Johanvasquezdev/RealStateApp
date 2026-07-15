using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.DTOs.PunchCard;

public class FilterPunchCardDto
{
    [Display(Name = "Sesion")]
    public int? SesionId { get; set; }

    [Display(Name = "Buscar Nombre")]
    public string? NombreEmpleado { get; set; }

    [Display(Name = "Solo Licencias")]
    public bool SoloLicencias { get; set; }

    [Range(1, 100)]
    public int Pagina { get; set; } = 1;

    [Range(1, 200)]
    public int TamanoPagina { get; set; } = 25;
}
