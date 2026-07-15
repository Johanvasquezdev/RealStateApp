using RealEstateApp.Core.Application.ViewModels.AgentChats;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface ChatService
{
    Task<List<AgentChatSummaryViewModel>> GetConversacionesByPropertyAsync(int propertyId, string agenteId);
    Task<AgentChatDetailViewModel> GetConversacionDetalleAsync(int propertyId, string clienteId, string agenteId);
    Task EnviarMensajeAsync(AgentSendMessageViewModel vm, string senderId);
}
