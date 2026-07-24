using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Properties;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Enums;

namespace RealEstateApp.Core.Application.Services;

public class ClientFavoriteService : IClientFavoriteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public ClientFavoriteService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task ToggleFavoriteAsync(int propertyId, string clientId)
    {
        var repo = _unitOfWork.Repository<FavoriteProperty>();
        var favorite = await repo.FirstOrDefaultAsync(f => f.PropertyId == propertyId && f.ClientId == clientId);

        if (favorite != null)
        {
            await repo.DeleteAsync(favorite);
        }
        else
        {
            await repo.AddAsync(new FavoriteProperty
            {
                PropertyId = propertyId,
                ClientId = clientId
            });
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<ClientPropertyViewModel>> GetClientFavoritesAsync(string clientId)
    {
        var favoriteRepo = _unitOfWork.Repository<FavoriteProperty>();
        var favorites = await favoriteRepo.FindWithIncludesAsync(
            f => f.ClientId == clientId, 
            "Property", "Property.PropertyType", "Property.SaleType", "Property.Images");

        var availableProperties = favorites
            .Select(f => f.Property)
            .Where(p => p.Status == PropertyStatus.Available)
            .ToList();

        var activeAgentProperties = new List<Property>();
        foreach (var property in availableProperties)
        {
            var agentUser = await _userService.FindByIdAsync(property.AgentId);
            if (agentUser is not null && agentUser.IsActive)
                activeAgentProperties.Add(property);
        }

        var vms = _mapper.Map<List<ClientPropertyViewModel>>(activeAgentProperties);
        
        foreach (var vm in vms)
        {
            var prop = activeAgentProperties.First(p => p.Id == vm.Id);
            
            var url = prop.Images?.FirstOrDefault()?.ImageUrl ?? "";
            vm.MainImageUrl = string.IsNullOrEmpty(url) ? "" : (url.StartsWith("http") || url.StartsWith("/") ? url : $"/Images/properties/{url}");
            vm.IsFavorite = true;

            var agentUser = await _userService.FindByIdAsync(prop.AgentId);
            if (agentUser != null)
            {
                vm.AgentName = $"{agentUser.FirstName} {agentUser.LastName}";
                vm.AgentPhotoUrl = agentUser.ProfilePictureUrl ?? "";
            }
        }

        return vms;
    }
}



