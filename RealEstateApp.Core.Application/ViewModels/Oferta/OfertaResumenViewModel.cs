namespace RealEstateApp.Core.Application.ViewModels.Oferta;

public class OfertaResumenViewModel
{
    public string ClienteId { get; set; } = null!;
    public string ClienteNombre { get; set; } = null!;
    public int CantidadOfertas { get; set; }
    public double UltimaOferta { get; set; }
    public string Estado { get; set; } = null!;
}
