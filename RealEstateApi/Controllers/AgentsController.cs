using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Agents;
using RealEstateApp.Core.Application.ViewModels.AgentProperties;

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

    [HttpGet("list")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AgentListViewModel>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        var agentes = await _agenteService.GetActiveAgentsAsync(null);
        if (agentes.Count == 0) return NoContent();
        return Ok(agentes);
    }

    [HttpGet("getbyid/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgentListViewModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(string id)
    {
        var agente = await _agenteService.GetAgentByIdAsync(id);
        if (agente is null) return NotFound();
        return Ok(agente);
    }

    [HttpGet("getagentproperty")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AgentPropertyViewModel>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProperties(string id)
    {
        var agente = await _agenteService.GetAgentByIdAsync(id);
        if (agente is null) return NotFound();

        var propiedades = await _agenteService.GetPropertiesByAgentAsync(id);
        if (propiedades.Count == 0) return NoContent();

        return Ok(propiedades);
    }


    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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


