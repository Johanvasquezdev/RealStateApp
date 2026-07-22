using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Properties;
using System.Security.Claims;

namespace RealEstateApp.Areas.Client.Controllers;

[Area("Client")]
[Authorize(Roles = "Cliente")]
public class HomeController : Controller
{
    private readonly IClientPropertyService _clientPropertyService;
    private readonly IPropertyTypeService _propertyTypeService;

    public HomeController(IClientPropertyService clientPropertyService, IPropertyTypeService propertyTypeService)
    {
        _clientPropertyService = clientPropertyService;
        _propertyTypeService = propertyTypeService;
    }

    public async Task<IActionResult> Index(ClientFilterPropertyViewModel filter)
    {
        var clientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var properties = await _clientPropertyService.GetPropertiesWithFiltersAsync(filter, clientId);
        ViewBag.PropertyTypes = await _propertyTypeService.GetAllViewModel();
        ViewBag.Filter = filter;

        return View("~/Areas/Client/Views/ClientProperty/Index.cshtml", properties);
    }
}
