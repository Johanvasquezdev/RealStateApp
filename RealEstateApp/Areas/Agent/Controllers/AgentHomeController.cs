using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Areas.Agent.Controllers;

[Area("Agent")]
[Authorize(Roles = "Agente")]
public class AgentHomeController : Controller
{
    private readonly IPropertyService _propertyService;

    public AgentHomeController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        var properties = await _propertyService.GetPropertiesByAgentAsync(userId);
        return View(properties);
    }
}

