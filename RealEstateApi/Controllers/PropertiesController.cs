using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs.Properties;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Enums;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Desarrollador")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertiesController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _propertyService.GetApiPropertiesAsync();

        if (result.Count == 0)
            return NoContent();

        return Ok(result);
    }

    [HttpGet("getbyid/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _propertyService.GetApiPropertyByIdAsync(id);

        if (result == null)
            return NotFound(new { error = "Propiedad no encontrada." });

        return Ok(result);
    }

    [HttpGet("getbycode/{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var result = await _propertyService.GetApiPropertyByCodeAsync(code);

        if (result == null)
            return NotFound(new { error = "Propiedad no encontrada." });

        return Ok(result);
    }
}

