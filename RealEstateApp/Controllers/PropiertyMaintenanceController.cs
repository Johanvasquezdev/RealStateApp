using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.AgentProperties;

namespace RealEstateApp.Controllers;

[Authorize(Roles = "Agente")]
public class PropiertyMaintenanceController : Controller
{
    private readonly IPropertyService _propiedadService;

    public PropiertyMaintenanceController(IPropertyService propiedadService)
    {
        _propiedadService = propiedadService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        var propiedades = await _propiedadService.GetPropiedadesByAgenteAsync(userId);
        return View(propiedades);
    }

    public IActionResult Create()
    {
        return View(new AgentPropertySaveViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(AgentPropertySaveViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        await _propiedadService.CreateAsync(vm, userId);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        var vm = await _propiedadService.GetPropiedadForEditAsync(id, userId);
        if (vm is null) return NotFound();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(AgentPropertySaveViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        await _propiedadService.UpdateAsync(vm, userId);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        var vm = await _propiedadService.GetPropiedadForEditAsync(id, userId);
        if (vm is null) return NotFound();
        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        await _propiedadService.DeleteAsync(id, userId);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Detail(int id)
    {
        var vm = await _propiedadService.GetPropiedadDetailAsync(id);
        if (vm is null) return NotFound();
        return View(vm);
    }
}
