using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using System.Security.Claims;

namespace RealEstateApp.Areas.Client.Controllers;

[Area("Client")]
[Authorize(Roles = "Cliente")]
public class ClientFavoriteController : Controller
{
    private readonly IClientFavoriteService _clientFavoriteService;

    public ClientFavoriteController(IClientFavoriteService clientFavoriteService)
    {
        _clientFavoriteService = clientFavoriteService;
    }

    public async Task<IActionResult> Index()
    {
        var clientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(clientId)) return RedirectToAction("Login", "Account");

        var favorites = await _clientFavoriteService.GetClientFavoritesAsync(clientId);
        return View(favorites);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleFavorite(int id)
    {
        var clientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(clientId)) return RedirectToAction("Login", "Account");

        await _clientFavoriteService.ToggleFavoriteAsync(id, clientId);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(new { success = true });
        }

        return Redirect(Request.Headers["Referer"].ToString());
    }
}
