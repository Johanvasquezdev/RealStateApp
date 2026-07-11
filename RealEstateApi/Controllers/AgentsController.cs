using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces.Services;

namespace RealEstateApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgentsController : ControllerBase
{
    private readonly IAgenteService _agenteService;

    public AgentsController(IAgenteService agenteService)
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
