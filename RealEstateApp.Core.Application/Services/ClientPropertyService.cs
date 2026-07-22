using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Properties;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Enums;
using RealEstateApp.Core.Domain.Common;
using Microsoft.Extensions.Caching.Memory;

namespace RealEstateApp.Core.Application.Services;

public class ClientPropertyService : IClientPropertyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IMemoryCache _memoryCache;

    public ClientPropertyService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserService userService,
        IMemoryCache memoryCache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userService = userService;
        _memoryCache = memoryCache;
    }

    #region List properties
    public async Task<List<ClientPropertyViewModel>> GetPropertiesWithFiltersAsync(ClientFilterPropertyViewModel filters, string? userId = null)
    {
        var repo = _unitOfWork.Repository<Property>();
        var properties = await repo.FindWithIncludesAsync(
            p => p.Status == PropertyStatus.Available,
            "PropertyType", "SaleType", "Images");

        if (!string.IsNullOrEmpty(filters.AgentId))
            properties = properties.Where(p => p.AgentId == filters.AgentId).ToList();

        if (filters.PropertyTypeId.HasValue)
            properties = properties.Where(p => p.PropertyTypeId == filters.PropertyTypeId.Value).ToList();
            
        if (filters.MinPrice.HasValue)
            properties = properties.Where(p => p.Price >= (double)filters.MinPrice.Value).ToList();
            
        if (filters.MaxPrice.HasValue)
            properties = properties.Where(p => p.Price <= (double)filters.MaxPrice.Value).ToList();
            
        if (filters.Rooms.HasValue)
            properties = filters.Rooms.Value == 4 ? properties.Where(p => p.Rooms >= 4).ToList() : properties.Where(p => p.Rooms == filters.Rooms.Value).ToList();
            
        if (filters.Bathrooms.HasValue)
            properties = filters.Bathrooms.Value == 4 ? properties.Where(p => p.Bathrooms >= 4).ToList() : properties.Where(p => p.Bathrooms == filters.Bathrooms.Value).ToList();
            
        if (!string.IsNullOrWhiteSpace(filters.Code))
            properties = properties.Where(p => p.Code != null && p.Code.Contains(filters.Code.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

        properties = properties.OrderByDescending(p => p.Created).ToList();

        var viewModels = _mapper.Map<List<ClientPropertyViewModel>>(properties);

        List<int> userFavoritePropertyIds = new();
        if (!string.IsNullOrEmpty(userId))
        {
            var favRepo = _unitOfWork.Repository<FavoriteProperty>();
            var userFavs = await favRepo.FindAsync(f => f.ClientId == userId);
            userFavoritePropertyIds = userFavs.Select(f => f.PropertyId).ToList();
        }

        var filteredViewModels = new List<ClientPropertyViewModel>();
        foreach (var vm in viewModels)
        {
            var prop = properties.First(p => p.Id == vm.Id);
            var url = prop.Images.FirstOrDefault()?.ImageUrl ?? "";
            vm.MainImageUrl = string.IsNullOrEmpty(url) ? "" : (url.StartsWith("http") || url.StartsWith("/") ? url : $"/Images/properties/{url}");

            if (!_memoryCache.TryGetValue($"Agent_{prop.AgentId}", out DTOs.UserDto? agentUser))
            {
                agentUser = await _userService.FindByIdAsync(prop.AgentId);
                if (agentUser != null)
                {
                    _memoryCache.Set($"Agent_{prop.AgentId}", agentUser, TimeSpan.FromMinutes(30));
                }
            }

            if (agentUser == null || !agentUser.IsActive)
            {
                continue;
            }

            vm.AgentName = $"{agentUser.FirstName} {agentUser.LastName}";
            vm.AgentPhotoUrl = agentUser.ProfilePictureUrl ?? "";
            vm.IsFavorite = userFavoritePropertyIds.Contains(vm.Id);
            
            filteredViewModels.Add(vm);
        }

        return filteredViewModels;
    }
    #endregion

    #region Property details
    public async Task<ClientPropertyDetailViewModel?> GetPropertyDetailAsync(int id, string? clientId = null)
    {
        Console.WriteLine($"[DEBUG] GetPropertyDetailAsync called for id {id}");
        var repo = _unitOfWork.Repository<Property>();
        var prop = await repo.FirstOrDefaultWithIncludesAsync(
            p => p.Id == id,
            "PropertyType", "SaleType", "Images", "PropertyImprovements.Improvement");

        if (prop == null) 
        {
            Console.WriteLine($"[DEBUG] Property {id} not found in DB.");
            return null;
        }

        Console.WriteLine($"[DEBUG] Property {id} found. Status is {prop.Status}. AgentId is {prop.AgentId}");

        var vm = _mapper.Map<ClientPropertyDetailViewModel>(prop);
        
        vm.ImageUrls = prop.Images.Select(i => string.IsNullOrEmpty(i.ImageUrl) ? "" : (i.ImageUrl.StartsWith("http") || i.ImageUrl.StartsWith("/") ? i.ImageUrl : $"/Images/properties/{i.ImageUrl}")).ToList();
        vm.Improvements = prop.PropertyImprovements.Select(pi => pi.Improvement.Name).ToList();

        var agentUser = await _userService.FindByIdAsync(prop.AgentId);
        if (agentUser == null)
        {
            Console.WriteLine($"[DEBUG] Agent {prop.AgentId} not found for property {id}.");
            return null; 
        }
        if (!agentUser.IsActive)
        {
            Console.WriteLine($"[DEBUG] Agent {prop.AgentId} is inactive. Hiding property {id}.");
            return null;
        }

        vm.AgentName = $"{agentUser.FirstName} {agentUser.LastName}";
        vm.AgentEmail = agentUser.Email;
        vm.AgentPhone = agentUser.PhoneNumber ?? "";
        vm.AgentPhotoUrl = agentUser.ProfilePictureUrl ?? "";
        vm.AgentId = agentUser.Id;

        vm.CanMakeOffer = await CheckCanMakeOfferAsync(prop.Id, clientId);

        return vm;
    }

    private async Task<bool> CheckCanMakeOfferAsync(int propertyId, string? clientId)
    {
        if (string.IsNullOrEmpty(clientId)) return false;

        var offerRepo = _unitOfWork.Repository<Offer>();
        
        var hasAcceptedOffer = await offerRepo.AnyAsync(o => o.PropertyId == propertyId && o.Status == RealEstateApp.Core.Domain.Enums.OfferStatus.Accepted);
        if (hasAcceptedOffer) return false;

        var hasPendingOffer = await offerRepo.AnyAsync(o => o.PropertyId == propertyId && o.ClientId == clientId && o.Status == RealEstateApp.Core.Domain.Enums.OfferStatus.Pending);
        if (hasPendingOffer) return false;

        return true;
    }
    #endregion
}



