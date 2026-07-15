namespace RealEstateApp.Core.Application.DTOs.PunchCard;

public class ResultimportationPunchCardDto
{
    public int SesionId { get; set; }
    public int TotalFilas { get; set; }
    public int RegistrosInsertados { get; set; }
    public int DuplicadosOmitidos { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public List<string> Errores { get; set; } = new();
}
