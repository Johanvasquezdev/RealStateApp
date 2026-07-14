using RealEstateApp.Core.Application.ViewModels.AgentOffers;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IOfertaService
{
    Task<List<AgentOfferSummaryViewModel>> GetOfertasResumenByPropertyAsync(int propertyId, string agenteId);
    Task<List<AgentOfferDetailViewModel>> GetOfertasByClientePropertyAsync(int propertyId, string clienteId, string agenteId);
    Task AceptarOfertaAsync(int ofertaId, string agenteId);
    Task RechazarOfertaAsync(int ofertaId, string agenteId);
}
