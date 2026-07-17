using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces.Services;

namespace RealEstateApp.Controllers;

[Authorize(Roles = "Agente")]
public class AgentHomeController : Controller
{
    private readonly IPropertyService _propiedadService;

    public AgentHomeController(IPropertyService propiedadService)
    {
        _propiedadService = propiedadService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        var propiedades = await _propiedadService.GetPropertiesByAgentAsync(userId);
        return View(propiedades);
    }
}
