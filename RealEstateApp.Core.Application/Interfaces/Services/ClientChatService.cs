using RealEstateApp.Core.Application.ViewModels.Messages;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface ClientChatService
{
    Task<int> SendMessageAsync(MessageSaveViewModel vm);
    // Para ver los chats del cliente
}
