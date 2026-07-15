using RealEstateApp.Core.Domain.Enums;

namespace RealEstateApp.Core.Application.DTOs.PunchCard;

public class RegistrationPunchCardDto
{
    public int Id { get; set; }
    public int SesionPunchCardId { get; set; }
    public string NombreEmpleado { get; set; } = string.Empty;
    public string? Departamento { get; set; }
    public DateTime Fecha { get; set; }
    public string? HoraEntrada { get; set; }
    public string? HoraSalida { get; set; }
    public decimal? HorasTrabajadas { get; set; }
    public EstadoPunchCard Estado { get; set; }
    public string? Observacion { get; set; }
    public bool NombreEditado { get; set; }
}
