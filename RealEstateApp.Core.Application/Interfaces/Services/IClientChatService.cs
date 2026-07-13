using RealEstateApp.Core.Application.ViewModels.Messages;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IClientChatService
{
    Task<int> SendMessageAsync(MessageSaveViewModel vm);
    // Para ver los chats del cliente
}
