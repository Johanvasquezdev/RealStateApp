using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.AgentChats;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Interfaces;
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

    public async Task<List<AgentChatSummaryViewModel>> GetConversacionesByPropertyAsync(int propertyId, string agenteId)
    {
        var propertyRepo = _unitOfWork.Repository<Property>();
        var property = await propertyRepo.GetByIdAsync(propertyId);
        if (property is null || property.AgentId != agenteId)
            return new();

        var messageRepo = _unitOfWork.Repository<Message>();
        var allMessages = await messageRepo.FindAsync(m => m.PropertyId == propertyId && m.ReceiverId == agenteId);
        var clientesIds = allMessages.Select(m => m.SenderId).Distinct().ToList();

        var result = new List<AgentChatSummaryViewModel>();
        foreach (var clienteId in clientesIds)
        {
            var client = await _userService.FindByIdAsync(clienteId);
            var clientMessages = (await messageRepo.FindAsync(m => m.PropertyId == propertyId && m.SenderId == clienteId))
                .OrderByDescending(m => m.Created);
            var ultimo = clientMessages.First();

            result.Add(new AgentChatSummaryViewModel
            {
                ClienteId = clienteId,
                ClienteNombre = client is not null ? $"{client.FirstName} {client.LastName}" : "Desconocido",
                UltimoMensaje = ultimo.Content,
                FechaUltimoMensaje = ultimo.Created,
                PropertyId = propertyId,
                PropertyCode = property.Code
            });
        }
        return result;
    }

    public async Task<AgentChatDetailViewModel> GetConversacionDetalleAsync(int propertyId, string clienteId, string agenteId)
    {
        var propertyRepo = _unitOfWork.Repository<Property>();
        var property = await propertyRepo.GetByIdAsync(propertyId);
        var client = await _userService.FindByIdAsync(clienteId);

        var messageRepo = _unitOfWork.Repository<Message>();
        var mensajes = await messageRepo.FindAsync(m =>
            m.PropertyId == propertyId &&
            ((m.SenderId == clienteId && m.ReceiverId == agenteId) ||
             (m.SenderId == agenteId && m.ReceiverId == clienteId)));
        mensajes = mensajes.OrderBy(m => m.Created).ToList();

        return new AgentChatDetailViewModel
        {
            ClienteId = clienteId,
            ClienteNombre = client is not null ? $"{client.FirstName} {client.LastName}" : "Desconocido",
            PropertyId = propertyId,
            PropertyCode = property?.Code ?? "",
            Mensajes = mensajes.Select(m => new MensajeViewModel
            {
                Content = m.Content,
                SenderId = m.SenderId,
                SenderNombre = m.SenderId == agenteId ? "Tú" : (client is not null ? $"{client.FirstName} {client.LastName}" : "Cliente"),
                Created = m.Created
            }).ToList()
        };
    }

    public async Task EnviarMensajeAsync(AgentSendMessageViewModel vm, string senderId)
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
