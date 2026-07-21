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
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly IUserService _userService;
    private readonly AutoMapper.IMapper _mapper;

    public PropertiesController(IGenericRepository<Property> propertyRepository, IUserService userService, AutoMapper.IMapper mapper)
    {
        _propertyRepository = propertyRepository;
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll()
    {
        var properties = await _propertyRepository.FindWithIncludesAsync(
            p => p.Status == PropertyStatus.Available,
            "PropertyType", "SaleType", "Images");

        var result = _mapper.Map<List<PropertyListApiResponse>>(properties);

        if (result.Count == 0)
            return NoContent();

        return Ok(result);
    }

    [HttpGet("getbyid/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var property = await _propertyRepository.FirstOrDefaultWithIncludesAsync(
            p => p.Id == id && p.Status == PropertyStatus.Available,
            "PropertyType", "SaleType", "Images", "PropertyImprovements.Improvement");

        if (property == null)
            return NotFound(new { error = "Propiedad no encontrada o no está disponible." });

        var agent = await _userService.FindByIdAsync(property.AgentId);

        var result = _mapper.Map<PropertyApiResponse>(property);
        result.AgentName = agent != null ? $"{agent.FirstName} {agent.LastName}" : "Unknown";

        return Ok(result);
    }

    [HttpGet("getbycode/{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var property = await _propertyRepository.FirstOrDefaultWithIncludesAsync(
            p => p.Code == code && p.Status == PropertyStatus.Available,
            "PropertyType", "SaleType", "Images", "PropertyImprovements.Improvement");

        if (property == null)
            return NotFound(new { error = "Propiedad no encontrada o no está disponible." });

        var agent = await _userService.FindByIdAsync(property.AgentId);

        var result = _mapper.Map<PropertyApiResponse>(property);
        result.AgentName = agent != null ? $"{agent.FirstName} {agent.LastName}" : "Unknown";

        return Ok(result);
    }
}

