using System.Diagnostics;
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
        if (User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("Agente"))
        {
            return RedirectToAction("Index", "AgentHome");
        }

        var properties = await _clientPropertyService.GetPropertiesWithFiltersAsync(filter);
        ViewBag.Filter = filter;
        return View(properties);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
