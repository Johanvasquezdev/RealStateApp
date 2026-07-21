using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Controllers;

public class AgentsController : Controller
{
    private readonly IAgentService _agenteService;

    public AgentsController(IAgentService agenteService)
    {
        _agenteService = agenteService;
    }

    public async Task<IActionResult> Index(string? nombreFilter)
    {
        var agentes = await _agenteService.GetActiveAgentsAsync(nombreFilter);
        ViewBag.NombreFilter = nombreFilter;
        return View(agentes);
    }

    public async Task<IActionResult> Details(string id)
    {
        var agente = await _agenteService.GetAgentByIdAsync(id);
        if (agente == null) return NotFound();
        var propiedades = await _agenteService.GetPropertiesByAgentAsync(id);
        ViewBag.Propiedades = propiedades;
        return View(agente);
    }
}


