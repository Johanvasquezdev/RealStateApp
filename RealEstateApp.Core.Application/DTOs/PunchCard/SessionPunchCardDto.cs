namespace RealEstateApp.Core.Application.DTOs.PunchCard;

public class SessionPunchCardDto
{
    public int Id { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public int TotalRegistros { get; set; }
    public int RegistrosProcesados { get; set; }
    public int RegistrosConError { get; set; }
    public string? UsuarioNombre { get; set; }
    public DateTime FechaSubida { get; set; }
}
