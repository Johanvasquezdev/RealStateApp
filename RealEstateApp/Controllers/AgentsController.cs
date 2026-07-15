using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces.Services;

namespace RealEstateApp.Controllers;

public class AgentsController : Controller
{
    private readonly AgentService _agenteService;

    public AgentsController(AgentService agenteService)
    {
        _agenteService = agenteService;
    }

    public async Task<IActionResult> Index(string? nombreFilter)
    {
        var agentes = await _agenteService.GetAgentesActivosAsync(nombreFilter);
        ViewBag.NombreFilter = nombreFilter;
        return View(agentes);
    }

    public async Task<IActionResult> Detalle(string id)
    {
        var agente = await _agenteService.GetAgenteByIdAsync(id);
        if (agente is null) return NotFound();
        var propiedades = await _agenteService.GetPropiedadesByAgenteAsync(id);
        ViewBag.Propiedades = propiedades;
        return View(agente);
    }
}
