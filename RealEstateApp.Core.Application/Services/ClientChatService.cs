using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Messages;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Enums;

namespace RealEstateApp.Core.Application.Services;

public class ClientChatService : IClientChatService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;
    private readonly IClientPropertyService _propertyService;

    public ClientChatService(IUnitOfWork unitOfWork, IUserService userService, IClientPropertyService propertyService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
        _propertyService = propertyService;
    }

    #region Send Message
    public async Task<int> SendMessageAsync(MessageSaveViewModel vm)
    {
        if (string.IsNullOrEmpty(vm.SenderId) || string.IsNullOrEmpty(vm.ReceiverId))
            throw new InvalidOperationException("Sender and Receiver IDs are required.");

        if (vm.SenderId == vm.ReceiverId)
            throw new InvalidOperationException("No puede enviarse un mensaje a sí mismo.");

        var receiverRoles = await _userService.GetRolesAsync(vm.ReceiverId);
        if (receiverRoles == null || !receiverRoles.Contains("Agente"))
            throw new InvalidOperationException("El destinatario no es un Agente válido.");

        var receiver = await _userService.FindByIdAsync(vm.ReceiverId);
        var propertyRepo = _unitOfWork.Repository<Property>();
        var property = await propertyRepo.GetByIdAsync(vm.PropertyId);
        if (receiver == null || !receiver.IsActive || property == null ||
            property.Status != PropertyStatus.Available || property.AgentId != vm.ReceiverId)
        {
            throw new InvalidOperationException("La propiedad no esta disponible o el agente no es su propietario.");
        }

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
        
        var messages = await msgRepo.FindAsync(m => m.SenderId == clientId || m.ReceiverId == clientId);

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
        var propRepo = _unitOfWork.Repository<Property>();
        var property = await propRepo.GetByIdAsync(propertyId);
        var agentUser = await _userService.FindByIdAsync(agentId);
        if (property == null || property.Status != PropertyStatus.Available || property.AgentId != agentId ||
            agentUser == null || !agentUser.IsActive)
        {
            throw new KeyNotFoundException("La conversacion solicitada no esta disponible.");
        }

        var msgRepo = _unitOfWork.Repository<Message>();
        var messages = await msgRepo.FindAsync(m => 
            m.PropertyId == propertyId && 
            ((m.SenderId == clientId && m.ReceiverId == agentId) || 
             (m.SenderId == agentId && m.ReceiverId == clientId)));

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

        result.AgentPropertyCount = await propRepo.CountAsync(p => p.AgentId == agentId);

        return result;
    }
    #endregion
}



