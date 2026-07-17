using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces.Services;

namespace RealEstateApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Developer")]
public class AgentsController : ControllerBase
{
    private readonly AgentService _agenteService;

    public AgentsController(AgentService agenteService)
    {
        _agenteService = agenteService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var agentes = await _agenteService.GetAgentesActivosAsync(null);
        return Ok(agentes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var agente = await _agenteService.GetAgenteByIdAsync(id);
        if (agente is null) return NotFound();
        return Ok(agente);
    }

    [HttpGet("{id}/properties")]
    public async Task<IActionResult> GetProperties(string id)
    {
        var propiedades = await _agenteService.GetPropiedadesByAgenteAsync(id);
        return Ok(propiedades);
    }
}
