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
    private readonly IPropertyTypeService _propertyTypeService;

    public ClientPropertyController(IClientPropertyService clientPropertyService, IPropertyTypeService propertyTypeService)
    {
        _clientPropertyService = clientPropertyService;
        _propertyTypeService = propertyTypeService;
    }

    public async Task<IActionResult> Index(ClientFilterPropertyViewModel filter)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var properties = await _clientPropertyService.GetPropertiesWithFiltersAsync(filter, userId);
        ViewBag.PropertyTypes = await _propertyTypeService.GetAllViewModel();
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

