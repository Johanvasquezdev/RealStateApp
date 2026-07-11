using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Cuenta;

namespace RealEstateApp.Controllers;

public class CuentaController : Controller
{
    private readonly IAuthService _authService;

    public CuentaController(IAuthService authService)
    {
        _authService = authService;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var result = await _authService.LoginAsync(vm);
        if (!result.Exito)
        {
            ModelState.AddModelError("", result.Mensaje);
            return View(vm);
        }
        if (result.Rol == "Agente")
            return RedirectToAction("Index", "AgenteHome");
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Registro()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Registro(RegisterViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var result = await _authService.RegisterAsync(vm);
        if (!result.Exito)
        {
            ModelState.AddModelError("", result.Mensaje);
            return View(vm);
        }
        TempData["Mensaje"] = result.Mensaje;
        return RedirectToAction("Login");
    }

    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }
}
