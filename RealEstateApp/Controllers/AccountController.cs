using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Accounts;

namespace RealEstateApp.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AccountController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
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
            return RedirectToAction("Index", "AgentHome");
        if (result.Rol == "Cliente")
            return RedirectToAction("Index", "ClientProperty");
            
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
        var origin = $"{Request.Scheme}://{Request.Host.Value}";
        var result = await _authService.RegisterAsync(vm, origin);
        if (!result.Exito)
        {
            ModelState.AddModelError("", result.Mensaje);
            return View(vm);
        }
        return RedirectToAction("RegistroCompletado");
    }

    public IActionResult RegistroCompletado()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ReenviarActivacion()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ReenviarActivacion(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError("", "Debe proporcionar un correo válido.");
            return View();
        }

        var origin = $"{Request.Scheme}://{Request.Host.Value}";
        var resultMessage = await _authService.ResendActivationEmailAsync(email, origin);

        ViewBag.Message = resultMessage;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            return RedirectToAction("Index", "Home");

        var result = await _userService.ConfirmEmailAsync(userId, token);
        if (result.Succeeded)
        {
            await _userService.SetActiveAsync(userId, true);
            TempData["Mensaje"] = "Su cuenta ha sido activada correctamente. Ahora puede iniciar sesión.";
        }
        else
        {
            TempData["Error"] = "Hubo un error al activar su cuenta.";
        }

        return RedirectToAction("Login");
    }

    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }
}


