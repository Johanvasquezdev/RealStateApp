using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Areas.Agent.Controllers;

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
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        var ofertas = await _ofertaService.GetOfferSummaryByPropertyAsync(propertyId, userId);
        ViewBag.PropertyId = propertyId;
        return View(ofertas);
    }

    public async Task<IActionResult> Detalle(int propertyId, string clienteId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        var ofertas = await _ofertaService.GetOffersByClientPropertyAsync(propertyId, clienteId, userId);
        if (!ofertas.Any()) return NotFound();
        ViewBag.PropertyId = propertyId;
        return View(ofertas);
    }

    public async Task<IActionResult> Aceptar(int id, int propertyId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        await _ofertaService.AcceptOfferAsync(id, userId);
        return RedirectToAction("Index", new { propertyId });
    }

    public async Task<IActionResult> Rechazar(int id, int propertyId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        await _ofertaService.RejectOfferAsync(id, userId);
        return RedirectToAction("Index", new { propertyId });
    }
}


