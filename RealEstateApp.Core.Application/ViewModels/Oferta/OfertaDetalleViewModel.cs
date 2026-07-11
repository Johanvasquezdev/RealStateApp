namespace RealEstateApp.Core.Application.ViewModels.Oferta;

public class OfertaDetalleViewModel
{
    public int Id { get; set; }
    public string ClienteNombre { get; set; } = null!;
    public double Amount { get; set; }
    public string Status { get; set; } = null!;
    public DateTime Created { get; set; }
}
