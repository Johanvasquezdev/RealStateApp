using AutoMapper;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Agents;
using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Application.Services;

public class ClientAgentService : Interfaces.Services.ClientAgentService
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ClientAgentService(IUserService userService, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _userService = userService;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ClientAgentViewModel>> GetAgentsAsync()
    {
        var users = await _userService.GetUsersInRoleAsync("Agente");
        // Filtrar activos si es necesario, o lo hace el UserService
        var activeAgents = users.Where(u => u.IsActive).ToList();
        
        var vms = new List<ClientAgentViewModel>();
        var propRepo = _unitOfWork.Repository<RealEstateApp.Core.Domain.Entities.Property>();

        foreach (var u in activeAgents.OrderBy(a => a.FirstName).ThenBy(a => a.LastName))
        {
            var propCount = await propRepo.CountAsync(p => p.AgentId == u.Id);
            vms.Add(new ClientAgentViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                ProfilePictureUrl = u.ProfilePictureUrl,
                PropertyCount = propCount
            });
        }

        return vms;
    }

    public async Task<ClientAgentViewModel?> GetAgentDetailAsync(string id)
    {
        var user = await _userService.FindByIdAsync(id);
        if (user == null || !user.IsActive) return null;

        var propRepo = _unitOfWork.Repository<RealEstateApp.Core.Domain.Entities.Property>();
        var propCount = await propRepo.CountAsync(p => p.AgentId == id);

        return new ClientAgentViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            ProfilePictureUrl = user.ProfilePictureUrl,
            PropertyCount = propCount
        };
    }
}
