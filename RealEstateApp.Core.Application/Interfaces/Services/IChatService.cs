using RealEstateApp.Core.Application.ViewModels.Chat;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IChatService
{
    Task<List<ConversacionResumenViewModel>> GetConversacionesByPropertyAsync(int propertyId, string agenteId);
    Task<ConversacionDetalleViewModel> GetConversacionDetalleAsync(int propertyId, string clienteId, string agenteId);
    Task EnviarMensajeAsync(EnviarMensajeViewModel vm, string senderId);
}
