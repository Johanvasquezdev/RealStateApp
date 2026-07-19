using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Messages;
using System.Security.Claims;

namespace RealEstateApp.Areas.Client.Controllers;

[Area("Client")]
[Authorize(Roles = "Cliente")]
public class ClientChatController : Controller
{
    private readonly IClientChatService _clientChatService;

    public ClientChatController(IClientChatService clientChatService)
    {
        _clientChatService = clientChatService;
    }

    public async Task<IActionResult> Index()
    {
        var clientId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var chats = await _clientChatService.GetConversationsAsync(clientId);
        return View(chats);
    }

    public async Task<IActionResult> Conversation(int propertyId, string agentId)
    {
        var clientId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var chat = await _clientChatService.GetConversationWithAgentAsync(clientId, agentId, propertyId);
        return View(chat);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(MessageSaveViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Conversation", new { propertyId = vm.PropertyId, agentId = vm.ReceiverId });
        }

        var clientId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        vm.SenderId = clientId;

        await _clientChatService.SendMessageAsync(vm);

        return RedirectToAction("Conversation", new { propertyId = vm.PropertyId, agentId = vm.ReceiverId });
    }
}

