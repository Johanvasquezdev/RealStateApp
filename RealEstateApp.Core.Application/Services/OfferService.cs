using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.AgentOffers;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Enums;

namespace RealEstateApp.Core.Application.Services;

public class OfferService : IOfferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;

    public OfferService(IUnitOfWork unitOfWork, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
    }

    public async Task<List<AgentOfferSummaryViewModel>> GetOfferSummaryByPropertyAsync(int propertyId, string agentId)
    {
        var propertyRepo = _unitOfWork.Repository<Property>();
        var property = await propertyRepo.GetByIdAsync(propertyId);
        if (property is null || property.AgentId != agentId)
            return new();

        var offerRepo = _unitOfWork.Repository<Offer>();
        var offers = await offerRepo.FindAsync(o => o.PropertyId == propertyId);
        var grouped = offers.GroupBy(o => o.ClientId);

        var result = new List<AgentOfferSummaryViewModel>();
        foreach (var group in grouped)
        {
            var client = await _userService.FindByIdAsync(group.Key);
            var last = group.OrderByDescending(o => o.Created).First();
            result.Add(new AgentOfferSummaryViewModel
            {
                ClientId = group.Key,
                ClientName = client is not null ? $"{client.FirstName} {client.LastName}" : "Desconocido",
                ClientPhoto = client?.ProfilePictureUrl,
                OfferCount = group.Count(),
                LatestOfferAmount = last.Amount,
                Status = last.Status.ToString()
            });
        }
        return result;
    }

    public async Task<List<AgentOfferDetailViewModel>> GetAllOffersByPropertyAsync(int propertyId, string agentId)
    {
        var propertyRepo = _unitOfWork.Repository<Property>();
        var property = await propertyRepo.GetByIdAsync(propertyId);
        if (property is null || property.AgentId != agentId)
            return new();

        var offerRepo = _unitOfWork.Repository<Offer>();
        var offers = await offerRepo.FindAsync(o => o.PropertyId == propertyId);
        offers = offers.OrderByDescending(o => o.Created).ToList();

        var result = new List<AgentOfferDetailViewModel>();
        foreach(var o in offers)
        {
            var client = await _userService.FindByIdAsync(o.ClientId);
            result.Add(new AgentOfferDetailViewModel
            {
                Id = o.Id,
                ClientName = client is not null ? $"{client.FirstName} {client.LastName}" : "Desconocido",
                Amount = o.Amount,
                Status = o.Status.ToString(),
                Created = o.Created
            });
        }
        return result;
    }

    public async Task<List<AgentOfferDetailViewModel>> GetOffersByClientPropertyAsync(int propertyId, string clientId, string agentId)
    {
        var propertyRepo = _unitOfWork.Repository<Property>();
        var property = await propertyRepo.GetByIdAsync(propertyId);
        if (property is null || property.AgentId != agentId)
            return new();

        var client = await _userService.FindByIdAsync(clientId);
        var offerRepo = _unitOfWork.Repository<Offer>();
        var offers = await offerRepo.FindAsync(o => o.PropertyId == propertyId && o.ClientId == clientId);
        offers = offers.OrderByDescending(o => o.Created).ToList();

        return offers.Select(o => new AgentOfferDetailViewModel
        {
            Id = o.Id,
            ClientName = client is not null ? $"{client.FirstName} {client.LastName}" : "Desconocido",
            Amount = o.Amount,
            Status = o.Status.ToString(),
            Created = o.Created
        }).ToList();
    }

    #region AcceptOfferAsync
    public async Task AcceptOfferAsync(int offerId, string agentId)
    {
        using var tx = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var offerRepo = _unitOfWork.Repository<Offer>();
            var propertyRepo = _unitOfWork.Repository<Property>();

            var offer = await offerRepo.FirstOrDefaultWithIncludesAsync(
                o => o.Id == offerId, "Property");
            if (offer is null || offer.Property.AgentId != agentId || offer.Status != OfferStatus.Pending)
                return;

            offer.Status = OfferStatus.Accepted;
            await offerRepo.UpdateAsync(offer);

            offer.Property.Status = PropertyStatus.Sold;
            await propertyRepo.UpdateAsync(offer.Property);

            await _unitOfWork.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
    #endregion

    #region RejectOfferAsync
    public async Task RejectOfferAsync(int offerId, string agentId)
    {
        var offerRepo = _unitOfWork.Repository<Offer>();
        var offer = await offerRepo.FirstOrDefaultWithIncludesAsync(
            o => o.Id == offerId, "Property");
        if (offer is null || offer.Property.AgentId != agentId || offer.Status != OfferStatus.Pending)
            return;

        offer.Status = OfferStatus.Rejected;
        await offerRepo.UpdateAsync(offer);
        await _unitOfWork.SaveChangesAsync();
    } 
    #endregion
}



