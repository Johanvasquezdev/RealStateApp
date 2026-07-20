using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Properties;

namespace RealEstateApp.Controllers;

public class HomeController : Controller
{
    private readonly IClientPropertyService _clientPropertyService;

    public HomeController(IClientPropertyService clientPropertyService)
    {
        _clientPropertyService = clientPropertyService;
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
        ViewBag.Filter = filter;
        return View(properties);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
