using AutoMapper;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Offers;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Application.Services;

public class ClientOfferService : Interfaces.Services.ClientOfferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;

    public ClientOfferService(IUnitOfWork unitOfWork, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
    }

    #region Create Offer
    public async Task<int> CreateOfferAsync(OfferSaveViewModel vm)
    {
        var repo = _unitOfWork.Repository<Offer>();
        
        // Check if there is already a pending offer from this client
        bool hasPending = await repo.AnyAsync(o => o.PropertyId == vm.PropertyId && o.ClientId == vm.ClientId && o.Status == RealEstateApp.Core.Domain.Enums.OfferStatus.Pendiente);
        if (hasPending) throw new Exception("Ya tienes una oferta pendiente para esta propiedad.");

        var entity = new Offer
        {
            PropertyId = vm.PropertyId,
            ClientId = vm.ClientId,
            Amount = (double)vm.Amount,
            Status = RealEstateApp.Core.Domain.Enums.OfferStatus.Pendiente,
            Created = DateTime.UtcNow
        };

        await repo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
    #endregion

    #region List Offers
    public async Task<List<ClientOfferListViewModel>> GetClientOffersAsync(string clientId)
    {
        var offerRepo = _unitOfWork.Repository<Offer>();
        var propertyRepo = _unitOfWork.Repository<Property>();
        
        var offers = await offerRepo.FindAsync(o => o.ClientId == clientId);
        var result = new List<ClientOfferListViewModel>();

        foreach (var offer in offers.OrderByDescending(o => o.Created))
        {
            var prop = await propertyRepo.FirstOrDefaultWithIncludesAsync(p => p.Id == offer.PropertyId, "Images");
            if (prop == null) continue;

            var agentUser = await _userService.FindByIdAsync(prop.AgentId);

            result.Add(new ClientOfferListViewModel
            {
                OfferId = offer.Id,
                PropertyId = prop.Id,
                PropertyName = prop.PropertyType != null ? prop.PropertyType.Name : "Propiedad",
                PropertyCode = prop.Code,
                PropertyImageUrl = prop.Images.FirstOrDefault()?.ImageUrl ?? "",
                Amount = (decimal)offer.Amount,
                Status = offer.Status.ToString(),
                CreatedAt = offer.Created,
                AgentName = agentUser != null ? $"{agentUser.FirstName} {agentUser.LastName}" : "",
                AgentEmail = agentUser?.Email ?? "",
                AgentPhotoUrl = agentUser?.ProfilePictureUrl
            });
        }

        return result;
    }
    #endregion

    #region Withdraw Offer
    public async Task WithdrawOfferAsync(int offerId, string clientId)
    {
        var repo = _unitOfWork.Repository<Offer>();
        var offer = await repo.FirstOrDefaultAsync(o => o.Id == offerId && o.ClientId == clientId);
        
        if (offer != null && offer.Status == RealEstateApp.Core.Domain.Enums.OfferStatus.Pendiente)
        {
            await repo.DeleteAsync(offer);
            await _unitOfWork.SaveChangesAsync();
        }
    }
    #endregion
}
