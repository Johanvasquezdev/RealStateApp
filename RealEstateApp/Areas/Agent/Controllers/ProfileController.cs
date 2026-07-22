using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.AgentProfile;

namespace RealEstateApp.Areas.Agent.Controllers;

[Area("Agent")]
[Authorize(Roles = "Agente")]
public class ProfileController : Controller
{
    private readonly IProfileService _perfilService;

    public ProfileController(IProfileService perfilService)
    {
        _perfilService = perfilService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        var vm = await _perfilService.GetProfileAsync(userId);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AgentProfileViewModel vm)
    {
        if (!ModelState.IsValid) return View("Index", vm);
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        await _perfilService.UpdateProfileAsync(userId, vm);
        TempData["Exito"] = "Perfil actualizado correctamente.";
        return RedirectToAction("Index");
    }
}


