using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Chat;

namespace RealEstateApp.Controllers;

[Authorize(Roles = "Agente")]
public class ChatController : Controller
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task<IActionResult> Index(int propertyId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        var conversaciones = await _chatService.GetConversacionesByPropertyAsync(propertyId, userId);
        ViewBag.PropertyId = propertyId;
        return View(conversaciones);
    }

    public async Task<IActionResult> Conversacion(int propertyId, string clienteId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        var vm = await _chatService.GetConversacionDetalleAsync(propertyId, clienteId, userId);
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> EnviarMensaje(EnviarMensajeViewModel vm)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        await _chatService.EnviarMensajeAsync(vm, userId);
        return RedirectToAction("Conversacion", new { propertyId = vm.PropertyId, clienteId = vm.ReceiverId });
    }
}
