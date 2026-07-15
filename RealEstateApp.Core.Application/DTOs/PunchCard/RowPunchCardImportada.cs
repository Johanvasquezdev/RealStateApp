namespace RealEstateApp.Core.Application.DTOs.PunchCard;

public class RowPunchCardImportada
{
    public string NombreEmpleado { get; set; } = string.Empty;
    public string? Departamento { get; set; }
    public DateTime Fecha { get; set; }
    public string? HoraEntrada { get; set; }
    public string? HoraSalida { get; set; }
    public decimal? HorasTrabajadas { get; set; }
}
