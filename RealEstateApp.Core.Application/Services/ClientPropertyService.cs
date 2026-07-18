using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Properties;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Enums;
using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Application.Services;

public class ClientPropertyService : IClientPropertyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public ClientPropertyService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userService = userService;
    }

    #region List properties
    public async Task<List<ClientPropertyViewModel>> GetPropertiesWithFiltersAsync(ClientFilterPropertyViewModel filters)
    {
        var repo = _unitOfWork.Repository<Property>();
        var properties = await repo.FindWithIncludesAsync(
            p => p.Status == PropertyStatus.Available,
            "PropertyType", "SaleType", "Images", "Agent");

        // Apply filters
        if (filters.PropertyTypeId.HasValue)
            properties = properties.Where(p => p.PropertyTypeId == filters.PropertyTypeId.Value).ToList();
            
        if (filters.MinPrice.HasValue)
            properties = properties.Where(p => p.Price >= (double)filters.MinPrice.Value).ToList();
            
        if (filters.MaxPrice.HasValue)
            properties = properties.Where(p => p.Price <= (double)filters.MaxPrice.Value).ToList();
            
        if (filters.Rooms.HasValue)
            properties = properties.Where(p => p.Rooms == filters.Rooms.Value).ToList();
            
        if (filters.Bathrooms.HasValue)
            properties = properties.Where(p => p.Bathrooms == filters.Bathrooms.Value).ToList();
            
        if (!string.IsNullOrWhiteSpace(filters.Code))
            properties = properties.Where(p => p.Code == filters.Code).ToList();

        // Sort: newest to oldest
        properties = properties.OrderByDescending(p => p.Created).ToList();

        var viewModels = _mapper.Map<List<ClientPropertyViewModel>>(properties);

        // Map extra fields
        foreach (var vm in viewModels)
        {
            var prop = properties.First(p => p.Id == vm.Id);
            vm.MainImageUrl = prop.Images.FirstOrDefault()?.ImageUrl ?? "";
            
            // Note: AgentName and AgentPhotoUrl can be fetched via IUserService if they aren't stored in the AppUser entity.
            // Assuming we fetch it here
            var agentUser = await _userService.FindByIdAsync(prop.AgentId);
            if (agentUser != null)
            {
                vm.AgentName = $"{agentUser.FirstName} {agentUser.LastName}";
                vm.AgentPhotoUrl = agentUser.ProfilePictureUrl ?? "";
            }
        }

        return viewModels;
    }
    #endregion

    #region Property details
    public async Task<ClientPropertyDetailViewModel?> GetPropertyDetailAsync(int id, string? clientId = null)
    {
        var repo = _unitOfWork.Repository<Property>();
        var prop = await repo.FirstOrDefaultWithIncludesAsync(
            p => p.Id == id && p.Status == PropertyStatus.Available,
            "PropertyType", "SaleType", "Images", "PropertyImprovements.Improvement");

        if (prop == null) return null;

        var vm = _mapper.Map<ClientPropertyDetailViewModel>(prop);
        
        vm.ImageUrls = prop.Images.Select(i => i.ImageUrl).ToList();
        vm.Improvements = prop.PropertyImprovements.Select(pi => pi.Improvement.Name).ToList();

        // Agent info
        var agentUser = await _userService.FindByIdAsync(prop.AgentId);
        if (agentUser != null)
        {
            vm.AgentName = $"{agentUser.FirstName} {agentUser.LastName}";
            vm.AgentEmail = agentUser.Email;
            vm.AgentPhone = agentUser.PhoneNumber ?? "";
            vm.AgentPhotoUrl = agentUser.ProfilePictureUrl ?? "";
            vm.AgentId = agentUser.Id;
        }

        vm.CanMakeOffer = await CheckCanMakeOfferAsync(prop.Id, clientId);

        return vm;
    }

    private async Task<bool> CheckCanMakeOfferAsync(int propertyId, string? clientId)
    {
        if (string.IsNullOrEmpty(clientId)) return false;

        var offerRepo = _unitOfWork.Repository<Offer>();
        
        // Cannot make offer if another offer is already accepted
        var hasAcceptedOffer = await offerRepo.AnyAsync(o => o.PropertyId == propertyId && o.Status == RealEstateApp.Core.Domain.Enums.OfferStatus.Accepted);
        if (hasAcceptedOffer) return false;

        // Cannot make offer if THIS client already has a pending offer
        var hasPendingOffer = await offerRepo.AnyAsync(o => o.PropertyId == propertyId && o.ClientId == clientId && o.Status == RealEstateApp.Core.Domain.Enums.OfferStatus.Pending);
        if (hasPendingOffer) return false;

        return true;
    }
    #endregion
}



