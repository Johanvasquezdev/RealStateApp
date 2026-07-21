using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using System.Security.Claims;

namespace RealEstateApp.Areas.Agent.Controllers;

[Area("Agent")]
[Authorize(Roles = "Agente")]
public class OfferController : Controller
{
    private readonly IOfferService _ofertaService;

    public OfferController(IOfferService ofertaService)
    {
        _ofertaService = ofertaService;
    }

    public async Task<IActionResult> Index(int propertyId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var ofertas = await _ofertaService.GetOfferSummaryByPropertyAsync(propertyId, userId);
        ViewBag.PropertyId = propertyId;
        return View(ofertas);
    }

    public async Task<IActionResult> Detalle(int propertyId, string clientId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var ofertas = await _ofertaService.GetOffersByClientPropertyAsync(propertyId, clientId, userId);
        ViewBag.PropertyId = propertyId;
        return View(ofertas);
    }

    public async Task<IActionResult> Aceptar(int id, int propertyId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        await _ofertaService.AcceptOfferAsync(id, userId);
        return RedirectToAction("Index", new { propertyId });
    }

    public async Task<IActionResult> Rechazar(int id, int propertyId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        await _ofertaService.RejectOfferAsync(id, userId);
        return RedirectToAction("Index", new { propertyId });
    }
}


