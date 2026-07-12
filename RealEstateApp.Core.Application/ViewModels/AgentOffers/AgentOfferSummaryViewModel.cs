namespace RealEstateApp.Core.Application.ViewModels.AgentOffers;

public class AgentOfferSummaryViewModel
{
    public string ClienteId { get; set; } = null!;
    public string ClienteNombre { get; set; } = null!;
    public int CantidadOfertas { get; set; }
    public double UltimaOferta { get; set; }
    public string Estado { get; set; } = null!;
}
