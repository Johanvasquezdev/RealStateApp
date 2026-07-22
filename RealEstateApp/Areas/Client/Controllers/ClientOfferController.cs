using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Offers;
using System.Security.Claims;

namespace RealEstateApp.Areas.Client.Controllers;

[Area("Client")]
[Authorize(Roles = "Cliente")]
public class ClientOfferController : Controller
{
    private readonly IClientOfferService _clientOfferService;

    public ClientOfferController(IClientOfferService clientOfferService)
    {
        _clientOfferService = clientOfferService;
    }

    public async Task<IActionResult> Index()
    {
        var clientId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var offers = await _clientOfferService.GetClientOffersAsync(clientId);
        return View(offers);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MakeOffer(OfferSaveViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Oferta inválida.";
            return RedirectToAction("Details", "ClientProperty", new { id = vm.PropertyId });
        }

        var clientId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        vm.ClientId = clientId;

        try
        {
            await _clientOfferService.CreateOfferAsync(vm);
            TempData["SuccessMessage"] = "Oferta enviada exitosamente.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction("Details", "ClientProperty", new { id = vm.PropertyId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> WithdrawOffer(int offerId)
    {
        var clientId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        await _clientOfferService.WithdrawOfferAsync(offerId, clientId);
        TempData["SuccessMessage"] = "Oferta retirada exitosamente.";
        return RedirectToAction(nameof(Index));
    }
}

