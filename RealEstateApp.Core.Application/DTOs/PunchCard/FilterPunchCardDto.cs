using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.DTOs.PunchCard;

public class FilterPunchCardDto
{
    [Display(Name = "Sesion")]
    public int? SessionId { get; set; }

    [Display(Name = "Buscar Nombre")]
    public string? EmployeeName { get; set; }

    [Display(Name = "Solo Licencias")]
    public bool OnLeaveOnly { get; set; }

    [Range(1, 100)]
    public int Page { get; set; } = 1;

    [Range(1, 200)]
    public int PageSize { get; set; } = 25;
}
