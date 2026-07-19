using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Properties;
using System.Security.Claims;

namespace RealEstateApp.Areas.Client.Controllers;

[Area("Client")]
[Authorize(Roles = "Cliente")]
public class ClientPropertyController : Controller
{
    private readonly IClientPropertyService _clientPropertyService;

    public ClientPropertyController(IClientPropertyService clientPropertyService)
    {
        _clientPropertyService = clientPropertyService;
    }

    // Default home page for clients
    public async Task<IActionResult> Index(ClientFilterPropertyViewModel filter)
    {
        var properties = await _clientPropertyService.GetPropertiesWithFiltersAsync(filter);
        ViewBag.Filter = filter;
        return View(properties);
    }

    public async Task<IActionResult> Details(int id)
    {
        var clientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var property = await _clientPropertyService.GetPropertyDetailAsync(id, clientId);
        if (property == null) return NotFound();

        return View(property);
    }
}

