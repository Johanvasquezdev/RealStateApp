using RealEstateApp.Core.Application.ViewModels.Messages;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface ClientChatService
{
    Task<int> SendMessageAsync(MessageSaveViewModel vm);
    Task<List<ClientChatListViewModel>> GetConversationsAsync(string clientId);
    Task<ClientChatConversationViewModel> GetConversationWithAgentAsync(string clientId, string agentId, int propertyId);
}
