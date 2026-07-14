using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.AgentProfile;

namespace RealEstateApp.Controllers;

[Authorize(Roles = "Agente")]
public class PerfilController : Controller
{
    private readonly IPerfilService _perfilService;

    public PerfilController(IPerfilService perfilService)
    {
        _perfilService = perfilService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        var vm = await _perfilService.GetPerfilAsync(userId);
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Index(AgentProfileViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        await _perfilService.UpdatePerfilAsync(userId, vm);
        TempData["Exito"] = "Perfil actualizado correctamente.";
        return RedirectToAction("Index");
    }
}
