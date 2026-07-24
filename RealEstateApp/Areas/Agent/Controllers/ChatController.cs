using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.AgentChats;

namespace RealEstateApp.Areas.Agent.Controllers;

[Area("Agent")]
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
        var conversaciones = await _chatService.GetConversationsByPropertyAsync(propertyId, userId);
        ViewBag.PropertyId = propertyId;
        return View(conversaciones);
    }

    public async Task<IActionResult> Conversation(int propertyId, string clientId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        var detalle = await _chatService.GetConversationDetailAsync(propertyId, clientId, userId);
        if (detalle == null) return NotFound();
        return View(detalle);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnviarMensaje(AgentSendMessageViewModel vm)
    {
        if (!ModelState.IsValid) return RedirectToAction("Conversation", new { propertyId = vm.PropertyId, clientId = vm.ReceiverId });
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        try
        {
            await _chatService.SendMessageAsync(vm, userId);
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Conversation", new { propertyId = vm.PropertyId, clientId = vm.ReceiverId });
    }
}


