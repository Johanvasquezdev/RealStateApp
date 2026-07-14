using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.AgentOffers;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Enums;

namespace RealEstateApp.Core.Application.Services;

public class OfferService : IOfertaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;

    public OfferService(IUnitOfWork unitOfWork, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
    }

    public async Task<List<AgentOfferSummaryViewModel>> GetOfertasResumenByPropertyAsync(int propertyId, string agenteId)
    {
        var propertyRepo = _unitOfWork.Repository<Property>();
        var property = await propertyRepo.GetByIdAsync(propertyId);
        if (property is null || property.AgentId != agenteId)
            return new();

        var offerRepo = _unitOfWork.Repository<Offer>();
        var ofertas = await offerRepo.FindAsync(o => o.PropertyId == propertyId);
        var grouped = ofertas.GroupBy(o => o.ClientId);

        var result = new List<AgentOfferSummaryViewModel>();
        foreach (var group in grouped)
        {
            var client = await _userService.FindByIdAsync(group.Key);
            var ultima = group.OrderByDescending(o => o.Created).First();
            result.Add(new AgentOfferSummaryViewModel
            {
                ClienteId = group.Key,
                ClienteNombre = client is not null ? $"{client.FirstName} {client.LastName}" : "Desconocido",
                CantidadOfertas = group.Count(),
                UltimaOferta = ultima.Amount,
                Estado = ultima.Status.ToString()
            });
        }
        return result;
    }

    public async Task<List<AgentOfferDetailViewModel>> GetOfertasByClientePropertyAsync(int propertyId, string clienteId, string agenteId)
    {
        var propertyRepo = _unitOfWork.Repository<Property>();
        var property = await propertyRepo.GetByIdAsync(propertyId);
        if (property is null || property.AgentId != agenteId)
            return new();

        var client = await _userService.FindByIdAsync(clienteId);
        var offerRepo = _unitOfWork.Repository<Offer>();
        var ofertas = await offerRepo.FindAsync(o => o.PropertyId == propertyId && o.ClientId == clienteId);
        ofertas = ofertas.OrderByDescending(o => o.Created).ToList();

        return ofertas.Select(o => new AgentOfferDetailViewModel
        {
            Id = o.Id,
            ClienteNombre = client is not null ? $"{client.FirstName} {client.LastName}" : "Desconocido",
            Amount = o.Amount,
            Status = o.Status.ToString(),
            Created = o.Created
        }).ToList();
    }

    #region AceptarOfertaAsync(
    public async Task AceptarOfertaAsync(int ofertaId, string agenteId)
    {
        using var tx = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var offerRepo = _unitOfWork.Repository<Offer>();
            var propertyRepo = _unitOfWork.Repository<Property>();

            var oferta = await offerRepo.FirstOrDefaultWithIncludesAsync(
                o => o.Id == ofertaId, "Property");
            if (oferta is null || oferta.Property.AgentId != agenteId || oferta.Status != OfferStatus.Pendiente)
                return;

            oferta.Status = OfferStatus.Aceptada;
            await offerRepo.UpdateAsync(oferta);

            var otrasPendientes = await offerRepo.FindAsync(
                o => o.PropertyId == oferta.PropertyId && o.Id != ofertaId && o.Status == OfferStatus.Pendiente);
            foreach (var o in otrasPendientes)
            {
                o.Status = OfferStatus.Rechazada;
                await offerRepo.UpdateAsync(o);
            }

            oferta.Property.Status = PropertyStatus.Vendida;
            await propertyRepo.UpdateAsync(oferta.Property);

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

    #region RechazarOfertaAsync
    public async Task RechazarOfertaAsync(int ofertaId, string agenteId)
    {
        var offerRepo = _unitOfWork.Repository<Offer>();
        var oferta = await offerRepo.FirstOrDefaultWithIncludesAsync(
            o => o.Id == ofertaId, "Property");
        if (oferta is null || oferta.Property.AgentId != agenteId || oferta.Status != OfferStatus.Pendiente)
            return;

        oferta.Status = OfferStatus.Rechazada;
        await offerRepo.UpdateAsync(oferta);
        await _unitOfWork.SaveChangesAsync();
    } 
    #endregion
}
