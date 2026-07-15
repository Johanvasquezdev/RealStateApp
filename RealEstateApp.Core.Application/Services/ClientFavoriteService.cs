using AutoMapper;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Properties;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Enums;

namespace RealEstateApp.Core.Application.Services;

public class ClientFavoriteService : Interfaces.Services.ClientFavoriteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ClientFavoriteService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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

        // Filtrar solo las disponibles o las que quiera ver
        var availableProperties = favorites
            .Select(f => f.Property)
            .Where(p => p.Status == PropertyStatus.Disponible)
            .ToList();

        var vms = _mapper.Map<List<ClientPropertyViewModel>>(availableProperties);
        
        foreach (var vm in vms)
        {
            var prop = availableProperties.First(p => p.Id == vm.Id);
            vm.MainImageUrl = prop.Images?.FirstOrDefault()?.ImageUrl ?? string.Empty;
        }

        return vms;
    }
}
