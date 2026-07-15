using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Domain.Entities;

public class SesionPunchCard : AuditableBaseEntity
{
    public string NombreArchivo { get; set; } = string.Empty;
    public int TotalRegistros { get; set; }
    public int RegistrosProcesados { get; set; }
    public int RegistrosConError { get; set; }
    public string? UsuarioId { get; set; }
    public string? UsuarioNombre { get; set; }
    public DateTime FechaSubida { get; set; } = DateTime.UtcNow;

    public ICollection<RegistroPunchCard> Registros { get; set; } = new List<RegistroPunchCard>();
}
