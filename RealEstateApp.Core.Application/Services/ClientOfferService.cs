using AutoMapper;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Offers;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Enums;

namespace RealEstateApp.Core.Application.Services;

public class ClientOfferService : Interfaces.Services.ClientOfferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ClientOfferService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<int> CreateOfferAsync(OfferSaveViewModel vm)
    {
        var repo = _unitOfWork.Repository<Offer>();
        
        // Regla de negocio: Un cliente no puede tener mÃ¡s de una oferta pendiente por propiedad al mismo tiempo.
        var hasPendingOffer = await repo.AnyAsync(o => o.ClientId == vm.ClientId && o.PropertyId == vm.PropertyId && o.Status == OfferStatus.Pendiente);
        if (hasPendingOffer)
        {
            throw new Exception("Ya tienes una oferta pendiente para esta propiedad.");
        }

        var offer = new Offer
        {
            Amount = (double)vm.Amount,
            PropertyId = vm.PropertyId,
            ClientId = vm.ClientId!,
            Status = OfferStatus.Pendiente
        };

        await repo.AddAsync(offer);
        await _unitOfWork.SaveChangesAsync();

        return offer.Id;
    }
}
