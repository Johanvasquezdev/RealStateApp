using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Enums;

namespace RealEstateApp.Core.Domain.Entities;

public class RegistroPunchCard : AuditableBaseEntity
{
    public int SesionPunchCardId { get; set; }
    public string NombreEmpleado { get; set; } = string.Empty;
    public string? Departamento { get; set; }
    public DateTime Fecha { get; set; }
    public string? HoraEntrada { get; set; }
    public string? HoraSalida { get; set; }
    public decimal? HorasTrabajadas { get; set; }
    public EstadoPunchCard Estado { get; set; } = EstadoPunchCard.Normal;
    public string? Observacion { get; set; }
    public bool NombreEditado { get; set; }
    public string? NombreOriginal { get; set; }

    public SesionPunchCard SesionPunchCard { get; set; } = null!;
}
