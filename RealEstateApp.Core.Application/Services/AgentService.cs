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
                PhoneNumber = user.PhoneNumber,
                ProfilePictureUrl = user.ProfilePictureUrl,
                PropertyCount = properties.Count,
                Activate = user.IsActive
            });
        }

        return result.OrderBy(a => a.FirstName).ThenBy(a => a.LastName).ToList();
    }

    public async Task<List<AgentListViewModel>> GetAllAgentsAsync()
    {
        var users = await _userService.GetUsersInRoleAsync("Agente");

        var result = new List<AgentListViewModel>();
        foreach (var user in users)
        {
            var properties = await _propertyRepository.FindAsync(p => p.AgentId == user.Id);
            result.Add(new AgentListViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfilePictureUrl = user.ProfilePictureUrl,
                PropertyCount = properties.Count,
                Activate = user.IsActive
            });
        }

        return result.OrderBy(a => a.FirstName).ThenBy(a => a.LastName).ToList();
    }

    public async Task<AgentListViewModel?> GetAgentByIdAsync(string id)
    {
        var user = await _userService.FindByIdAsync(id);
        if (user is null) return null;

        var roles = await _userService.GetRolesAsync(user.Id);
        if (!roles.Contains("Agente")) return null;

        if (!user.IsActive) return null;

        var properties = await _propertyRepository.FindAsync(p => p.AgentId == user.Id);

        return new AgentListViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            ProfilePictureUrl = user.ProfilePictureUrl,
            PropertyCount = properties.Count,
            Activate = user.IsActive
        };
    }

    public async Task<AgentListViewModel?> GetAgentByIdIncludingInactiveAsync(string id)
    {
        var user = await _userService.FindByIdAsync(id);
        if (user is null) return null;

        var roles = await _userService.GetRolesAsync(user.Id);
        if (!roles.Contains("Agente")) return null;

        var properties = await _propertyRepository.FindAsync(p => p.AgentId == user.Id);

        return new AgentListViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            ProfilePictureUrl = user.ProfilePictureUrl,
            PropertyCount = properties.Count,
            Activate = user.IsActive
        };
    }

    public async Task<List<AgentPropertyViewModel>> GetPropertiesByAgentAsync(string id)
    {
        var properties = await _propertyRepository.FindWithIncludesAsync(p => p.AgentId == id, "PropertyType", "SaleType", "Images");
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
            Status = p.Status.ToString(),
            MainImage = p.Images?.FirstOrDefault()?.ImageUrl
        }).ToList();
    }
}
