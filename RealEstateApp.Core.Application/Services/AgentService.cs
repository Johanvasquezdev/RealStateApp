using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Agents;
using RealEstateApp.Core.Application.ViewModels.AgentProperties;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Services;

public class AgentService : IAgentService
{
    private readonly IUserService _userService;
    private readonly IGenericRepository<Property> _propertyRepository;

    public AgentService(IUserService userService, IGenericRepository<Property> propertyRepository)
    {
        _userService = userService;
        _propertyRepository = propertyRepository;
    }

    public async Task<List<AgentListViewModel>> GetActiveAgentsAsync(string? nameFilter)
    {
        var users = await _userService.GetUsersInRoleAsync("Agente");
        var activos = users.Where(u => u.IsActive).AsEnumerable();

        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            var filtro = nameFilter.ToLower();
            activos = activos.Where(u =>
                u.FirstName.ToLower().Contains(filtro) ||
                u.LastName.ToLower().Contains(filtro));
        }

        var result = new List<AgentListViewModel>();
        foreach (var user in activos)
        {
            var properties = await _propertyRepository.FindAsync(p => p.AgentId == user.Id);
            result.Add(new AgentListViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl,
                PropertyCount = properties.Count
            });
        }

        return result;
    }

    public async Task<AgentListViewModel?> GetAgentByIdAsync(string id)
    {
        var user = await _userService.FindByIdAsync(id);
        if (user is null || !user.IsActive) return null;

        var properties = await _propertyRepository.FindAsync(p => p.AgentId == user.Id);

        return new AgentListViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            ProfilePictureUrl = user.ProfilePictureUrl,
            PropertyCount = properties.Count
        };
    }

    public async Task<List<AgentPropertyViewModel>> GetPropertiesByAgentAsync(string id)
    {
        var properties = await _propertyRepository.FindAsync(p => p.AgentId == id);
        return properties.Select(p => new AgentPropertyViewModel
        {
            Id = p.Id,
            Code = p.Code,
            Price = p.Price,
            Description = p.Description,
            LandSize = p.LandSize,
            Rooms = p.Rooms,
            Bathrooms = p.Bathrooms,
            PropertyType = p.PropertyType?.Name ?? "",
            SaleType = p.SaleType?.Name ?? "",
            Status = p.Status.ToString()
        }).ToList();
    }
}
