using RealEstateApp.Core.Application.ViewModels.Oferta;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IOfertaService
{
    Task<List<OfertaResumenViewModel>> GetOfertasResumenByPropertyAsync(int propertyId, string agenteId);
    Task<List<OfertaDetalleViewModel>> GetOfertasByClientePropertyAsync(int propertyId, string clienteId, string agenteId);
    Task AceptarOfertaAsync(int ofertaId, string agenteId);
    Task RechazarOfertaAsync(int ofertaId, string agenteId);
}
