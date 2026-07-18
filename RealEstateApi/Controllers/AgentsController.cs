using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Agents;

namespace RealEstateApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Desarrollador")]
public class AgentsController : ControllerBase
{
    private readonly IAgentService _agenteService;
    private readonly IUserManagementService _userManagementService;

    public AgentsController(IAgentService agenteService, IUserManagementService userManagementService)
    {
        _agenteService = agenteService;
        _userManagementService = userManagementService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var agentes = await _agenteService.GetActiveAgentsAsync(null);
        return Ok(agentes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var agente = await _agenteService.GetAgentByIdAsync(id);
        if (agente is null) return NotFound();
        return Ok(agente);
    }

    [HttpGet("{id}/properties")]
    public async Task<IActionResult> GetProperties(string id)
    {
        var propiedades = await _agenteService.GetPropertiesByAgentAsync(id);
        return Ok(propiedades);
    }


    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ChangeStatus(string id, AgentListViewModel request)
    {
        var result = await _userManagementService.ToggleAgentStatus(id, request.Activate);

        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.Contains("no existe")))
                return NotFound(new { error = "El agente solicitado no existe." });

            return BadRequest(new { error = "El estado enviado no es válido.", details = result.Errors });
        }

        return NoContent();
    }
}


