using RealEstateApp.Core.Application.ViewModels.AgentChats;

namespace RealEstateApp.Core.Application.Interfaces;

public interface IChatService
{
    Task<List<AgentChatSummaryViewModel>> GetConversationsByPropertyAsync(int propertyId, string agentId);
    Task<AgentChatDetailViewModel> GetConversationDetailAsync(int propertyId, string clientId, string agentId);
    Task SendMessageAsync(AgentSendMessageViewModel vm, string senderId);
}


