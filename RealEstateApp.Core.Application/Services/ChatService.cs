using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.AgentChats;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Services;

public class ChatService : IChatService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;

    public ChatService(IUnitOfWork unitOfWork, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
    }

    public async Task<List<AgentChatSummaryViewModel>> GetConversationsByPropertyAsync(int propertyId, string agentId)
    {
        var propertyRepo = _unitOfWork.Repository<Property>();
        var property = await propertyRepo.GetByIdAsync(propertyId);
        if (property is null || property.AgentId != agentId)
            return new();

        var messageRepo = _unitOfWork.Repository<Message>();
        var allMessages = await messageRepo.FindAsync(m => m.PropertyId == propertyId && m.ReceiverId == agentId);
        var clientIds = allMessages.Select(m => m.SenderId).Distinct().ToList();

        var result = new List<AgentChatSummaryViewModel>();
        foreach (var clientId in clientIds)
        {
            var client = await _userService.FindByIdAsync(clientId);
            var clientMessages = (await messageRepo.FindAsync(m => m.PropertyId == propertyId && m.SenderId == clientId))
                .OrderByDescending(m => m.Created);
            var last = clientMessages.First();

            result.Add(new AgentChatSummaryViewModel
            {
                ClientId = clientId,
                ClientName = client is not null ? $"{client.FirstName} {client.LastName}" : "Desconocido",
                LastMessage = last.Content,
                LastMessageDate = last.Created,
                PropertyId = propertyId,
                PropertyCode = property.Code
            });
        }
        return result;
    }

    public async Task<AgentChatDetailViewModel> GetConversationDetailAsync(int propertyId, string clientId, string agentId)
    {
        var propertyRepo = _unitOfWork.Repository<Property>();
        var property = await propertyRepo.GetByIdAsync(propertyId);
        var client = await _userService.FindByIdAsync(clientId);

        var messageRepo = _unitOfWork.Repository<Message>();
        var messages = await messageRepo.FindAsync(m =>
            m.PropertyId == propertyId &&
            ((m.SenderId == clientId && m.ReceiverId == agentId) ||
             (m.SenderId == agentId && m.ReceiverId == clientId)));
        messages = messages.OrderBy(m => m.Created).ToList();

        return new AgentChatDetailViewModel
        {
            ClientId = clientId,
            ClientName = client is not null ? $"{client.FirstName} {client.LastName}" : "Desconocido",
            PropertyId = propertyId,
            PropertyCode = property?.Code ?? "",
            Messages = messages.Select(m => new MessageViewModel
            {
                Content = m.Content,
                SenderId = m.SenderId,
                SenderName = m.SenderId == agentId ? "Tú" : (client is not null ? $"{client.FirstName} {client.LastName}" : "Cliente"),
                Created = m.Created
            }).ToList()
        };
    }

    public async Task SendMessageAsync(AgentSendMessageViewModel vm, string senderId)
    {
        var messageRepo = _unitOfWork.Repository<Message>();
        var msg = new Message
        {
            SenderId = senderId,
            ReceiverId = vm.ReceiverId,
            PropertyId = vm.PropertyId,
            Content = vm.Content
        };
        await messageRepo.AddAsync(msg);
        await _unitOfWork.SaveChangesAsync();
    }
}



