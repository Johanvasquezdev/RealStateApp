using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Improvement;

namespace RealEstateApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImprovementsController(IImprovementService service) : ControllerBase
{
    private readonly IImprovementService _service = service;

    [HttpGet("list")]
    [Authorize(Roles = "Administrador,Desarrollador")]
    public async Task<IActionResult> List()
    {
        var list = await _service.GetAllViewModel();
        if (list.Count == 0) return NoContent();
        return Ok(list);
    }

    [HttpGet("getbyid/{id}")]
    [Authorize(Roles = "Administrador,Desarrollador")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var vm = await _service.GetByIdViewModel(id);
            return Ok(vm);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "La mejora solicitada no existe." });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create([FromBody] SaveImprovementViewModel vm)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Los datos enviados no son válidos.", details = ModelState });

        var created = await _service.Add(vm);
        return StatusCode(201, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(int id, [FromBody] SaveImprovementViewModel vm)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Los datos enviados no son válidos.", details = ModelState });

        try
        {
            await _service.Update(vm, id);
            var updated = await _service.GetByIdViewModel(id);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "La mejora solicitada no existe." });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.Delete(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "La mejora solicitada no existe." });
        }
    }
}