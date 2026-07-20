using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Messages;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Application.Services;

public class ClientChatService : IClientChatService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;
    private readonly IClientPropertyService _propertyService; // for checking property info if needed

    public ClientChatService(IUnitOfWork unitOfWork, IUserService userService, IClientPropertyService propertyService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
        _propertyService = propertyService;
    }

    #region Send Message
    public async Task<int> SendMessageAsync(MessageSaveViewModel vm)
    {
        var repo = _unitOfWork.Repository<Message>();
        var entity = new Message
        {
            PropertyId = vm.PropertyId,
            SenderId = vm.SenderId,
            ReceiverId = vm.ReceiverId,
            Content = vm.Content,
            Created = DateTime.UtcNow
        };
        await repo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
    #endregion

    #region List Conversations
    public async Task<List<ClientChatListViewModel>> GetConversationsAsync(string clientId)
    {
        var msgRepo = _unitOfWork.Repository<Message>();
        
        // Find all messages where client is sender or receiver
        var messages = await msgRepo.FindAsync(m => m.SenderId == clientId || m.ReceiverId == clientId);

        // Group by Agent (the other person) and Property
        var conversations = messages
            .GroupBy(m => new { 
                AgentId = m.SenderId == clientId ? m.ReceiverId : m.SenderId, 
                PropertyId = m.PropertyId 
            })
            .ToList();

        var result = new List<ClientChatListViewModel>();

        foreach (var conv in conversations)
        {
            var latestMsg = conv.OrderByDescending(m => m.Created).First();
            var agentUser = await _userService.FindByIdAsync(conv.Key.AgentId);

            if (agentUser != null)
            {
                result.Add(new ClientChatListViewModel
                {
                    AgentId = agentUser.Id,
                    AgentName = $"{agentUser.FirstName} {agentUser.LastName}",
                    AgentPhotoUrl = agentUser.ProfilePictureUrl,
                    PropertyId = conv.Key.PropertyId,
                    LastMessage = latestMsg.Content,
                    LastMessageDate = latestMsg.Created
                });
            }
        }

        return result.OrderByDescending(c => c.LastMessageDate).ToList();
    }
    #endregion

    #region Get Conversation Details
    public async Task<ClientChatConversationViewModel> GetConversationWithAgentAsync(string clientId, string agentId, int propertyId)
    {
        var msgRepo = _unitOfWork.Repository<Message>();
        var messages = await msgRepo.FindAsync(m => 
            m.PropertyId == propertyId && 
            ((m.SenderId == clientId && m.ReceiverId == agentId) || 
             (m.SenderId == agentId && m.ReceiverId == clientId)));

        var agentUser = await _userService.FindByIdAsync(agentId);
        if (agentUser == null) return new ClientChatConversationViewModel();

        var result = new ClientChatConversationViewModel
        {
            AgentId = agentUser.Id,
            AgentName = $"{agentUser.FirstName} {agentUser.LastName}",
            AgentPhotoUrl = agentUser.ProfilePictureUrl,
            AgentTitle = "Agente Inmobiliario",
            PropertyId = propertyId,
            Messages = messages.OrderBy(m => m.Created).Select(m => new ChatBubbleViewModel
            {
                Content = m.Content,
                IsFromClient = m.SenderId == clientId,
                SentAt = m.Created
            }).ToList()
        };

        // We can get AgentPropertyCount by asking the user service or via unitofwork
        var propRepo = _unitOfWork.Repository<Property>();
        result.AgentPropertyCount = await propRepo.CountAsync(p => p.AgentId == agentId);

        return result;
    }
    #endregion
}



