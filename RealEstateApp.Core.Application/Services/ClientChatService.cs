using AutoMapper;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Messages;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Services;

public class ClientChatService : Interfaces.Services.ClientChatService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ClientChatService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<int> SendMessageAsync(MessageSaveViewModel vm)
    {
        var repo = _unitOfWork.Repository<Message>();

        // Necesitamos el AgentId de la propiedad para vincular el mensaje
        var propertyRepo = _unitOfWork.Repository<Property>();
        var property = await propertyRepo.GetByIdAsync(vm.PropertyId);
        if (property == null) throw new Exception("Propiedad no encontrada");

        var message = new Message
        {
            Content = vm.Content,
            PropertyId = vm.PropertyId,
            SenderId = vm.SenderId!,
            ReceiverId = property.AgentId
        };

        await repo.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();

        return message.Id;
    }
}
