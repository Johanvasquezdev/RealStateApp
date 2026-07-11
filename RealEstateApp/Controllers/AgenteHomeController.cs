using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces.Services;

namespace RealEstateApp.Controllers;

[Authorize(Roles = "Agente")]
public class AgenteHomeController : Controller
{
    private readonly IPropiedadService _propiedadService;

    public AgenteHomeController(IPropiedadService propiedadService)
    {
        _propiedadService = propiedadService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        var propiedades = await _propiedadService.GetPropiedadesByAgenteAsync(userId);
        return View(propiedades);
    }
}
