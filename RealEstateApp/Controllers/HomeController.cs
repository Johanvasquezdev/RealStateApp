using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Properties;

namespace RealEstateApp.Controllers;

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
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            if (User.IsInRole("Administrador"))
            {
                return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
            }

            if (User.IsInRole("Agente"))
            {
                return RedirectToAction("Index", "AgentHome", new { area = "Agent" });
            }

            // Si el usuario es Cliente, le permitimos ver el Home público.
        }

        var properties = await _clientPropertyService.GetPropertiesWithFiltersAsync(filter);
        ViewBag.PropertyTypes = await _propertyTypeService.GetAllViewModel();
        ViewBag.Filter = filter;
        return View(properties);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
