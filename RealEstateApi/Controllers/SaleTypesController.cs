using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.SaleType;

namespace RealEstateApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SaleTypesController(ISaleTypeService service) : ControllerBase
{
    private readonly ISaleTypeService _service = service;

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
            return NotFound(new { error = "El tipo de venta solicitado no existe." });
        }
    }

    [HttpPost("create")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create([FromBody] SaveSaleTypeViewModel vm)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Los datos enviados no son válidos.", details = ModelState });

        try
        {
            var created = await _service.Add(vm);
            return StatusCode(201, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("update/{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(int id, [FromBody] SaveSaleTypeViewModel vm)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Los datos enviados no son válidos.", details = ModelState });

        if (id != vm.Id)
        {
            vm.Id = id;
        }

        try
        {
            await _service.Update(vm, id);
            var updated = await _service.GetByIdViewModel(id);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "El tipo de venta solicitado no existe." });
        }
    }

    [HttpDelete("delete")]
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
            return NotFound(new { error = "El tipo de venta solicitado no existe." });
        }
    }
}