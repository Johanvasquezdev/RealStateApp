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

    public PropertiesController(IGenericRepository<Property> propertyRepository, IUserService userService)
    {
        _propertyRepository = propertyRepository;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var properties = await _propertyRepository.FindWithIncludesAsync(
            p => p.Status == PropertyStatus.Available,
            "PropertyType", "SaleType", "Images");

        var result = properties.Select(p => new PropertyListApiResponse
        {
            Id = p.Id,
            Code = p.Code,
            PropertyType = p.PropertyType.Name,
            SaleType = p.SaleType.Name,
            Price = (decimal)p.Price,
            LandSize = p.LandSize,
            Rooms = p.Rooms,
            Bathrooms = p.Bathrooms,
            MainImageUrl = p.Images.FirstOrDefault()?.ImageUrl ?? string.Empty
        }).ToList();

        if (result.Count == 0)
            return NoContent();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var property = await _propertyRepository.FirstOrDefaultWithIncludesAsync(
            p => p.Id == id && p.Status == PropertyStatus.Available,
            "PropertyType", "SaleType", "Images", "PropertyImprovements.Improvement");

        if (property == null)
            return NotFound(new { error = "Propiedad no encontrada o no está disponible." });

        var agent = await _userService.FindByIdAsync(property.AgentId);

        var result = new PropertyApiResponse
        {
            Id = property.Id,
            Code = property.Code,
            PropertyType = property.PropertyType.Name,
            SaleType = property.SaleType.Name,
            Price = (decimal)property.Price,
            LandSize = property.LandSize,
            Rooms = property.Rooms,
            Bathrooms = property.Bathrooms,
            Description = property.Description,
            Status = property.Status.ToString(),
            AgentId = property.AgentId,
            AgentName = agent != null ? $"{agent.FirstName} {agent.LastName}" : "Unknown",
            Images = property.Images.Select(i => i.ImageUrl).ToList(),
            Improvements = property.PropertyImprovements.Select(pi => pi.Improvement.Name).ToList()
        };

        return Ok(result);
    }

    [HttpGet("code/{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var property = await _propertyRepository.FirstOrDefaultWithIncludesAsync(
            p => p.Code == code && p.Status == PropertyStatus.Available,
            "PropertyType", "SaleType", "Images", "PropertyImprovements.Improvement");

        if (property == null)
            return NotFound(new { error = "Propiedad no encontrada o no está disponible." });

        var agent = await _userService.FindByIdAsync(property.AgentId);

        var result = new PropertyApiResponse
        {
            Id = property.Id,
            Code = property.Code,
            PropertyType = property.PropertyType.Name,
            SaleType = property.SaleType.Name,
            Price = (decimal)property.Price,
            LandSize = property.LandSize,
            Rooms = property.Rooms,
            Bathrooms = property.Bathrooms,
            Description = property.Description,
            Status = property.Status.ToString(),
            AgentId = property.AgentId,
            AgentName = agent != null ? $"{agent.FirstName} {agent.LastName}" : "Unknown",
            Images = property.Images.Select(i => i.ImageUrl).ToList(),
            Improvements = property.PropertyImprovements.Select(pi => pi.Improvement.Name).ToList()
        };

        return Ok(result);
    }
}

