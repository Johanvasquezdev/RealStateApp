using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces.Services;

namespace RealEstateApp.Controllers;

[Authorize(Roles = "Cliente")]
public class ClientAgentController : Controller
{
    private readonly IClientAgentService _clientAgentService;

    public ClientAgentController(IClientAgentService clientAgentService)
    {
        _clientAgentService = clientAgentService;
    }

    public async Task<IActionResult> Index(string? nombreFilter)
    {
        var agents = await _clientAgentService.GetAgentsAsync();
        if (!string.IsNullOrEmpty(nombreFilter))
        {
            agents = agents.Where(a => $"{a.FirstName} {a.LastName}".Contains(nombreFilter, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        ViewBag.NombreFilter = nombreFilter;
        return View(agents);
    }

    public async Task<IActionResult> Details(string id)
    {
        var agent = await _clientAgentService.GetAgentDetailAsync(id);
        if (agent == null) return NotFound();

        return View(agent);
    }
}
