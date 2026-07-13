using AutoMapper;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Agents;

namespace RealEstateApp.Core.Application.Services;

public class ClientAgentService : IClientAgentService
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public ClientAgentService(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<List<ClientAgentViewModel>> GetAgentsAsync()
    {
        var users = await _userService.GetUsersInRoleAsync("Agente");
        // Filtrar activos si es necesario, o lo hace el UserService
        var activeAgents = users.Where(u => u.IsActive).ToList();
        
        var vms = activeAgents.Select(u => new ClientAgentViewModel
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber,
            ProfilePictureUrl = u.ProfilePictureUrl
        }).ToList();

        return vms;
    }

    public async Task<ClientAgentViewModel?> GetAgentDetailAsync(string id)
    {
        var user = await _userService.FindByIdAsync(id);
        if (user == null || !user.IsActive) return null;

        return new ClientAgentViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            ProfilePictureUrl = user.ProfilePictureUrl
        };
    }
}
