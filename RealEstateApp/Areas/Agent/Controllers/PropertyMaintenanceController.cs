using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.AgentProperties;
using RealEstateApp.Core.Domain.Enums;
using System.Security.Claims;

namespace RealEstateApp.Areas.Agent.Controllers;

[Area("Agent")]
[Authorize(Roles = "Agente")]
public class PropertyMaintenanceController : Controller
{
    private readonly IPropertyService _propertyService;
    private readonly IPropertyTypeService _propertyTypeService;
    private readonly ISaleTypeService _saleTypeService;
    private readonly IImprovementService _improvementService;
    private readonly IChatService _chatService;
    private readonly IOfferService _offerService;

    public PropertyMaintenanceController(IPropertyService propertyService, IPropertyTypeService propertyTypeService, ISaleTypeService saleTypeService, IImprovementService improvementService, IChatService chatService, IOfferService offerService)
    {
        _propertyService = propertyService;
        _propertyTypeService = propertyTypeService;
        _saleTypeService = saleTypeService;
        _improvementService = improvementService;
        _chatService = chatService;
        _offerService = offerService;
    }

    public async Task<IActionResult> Index(string? filterCode)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var propiedades = await _propertyService.GetPropertiesByAgentAsync(userId);
        
        if (!string.IsNullOrWhiteSpace(filterCode))
        {
            propiedades = propiedades.Where(p => p.Code.Contains(filterCode, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        
        ViewBag.FilterCode = filterCode;
        return View(propiedades);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.PropertyTypes = await _propertyTypeService.GetAllViewModel();
        ViewBag.SaleTypes = await _saleTypeService.GetAllViewModel();
        ViewBag.Improvements = await _improvementService.GetAllViewModel();
        return View(new AgentPropertySaveViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AgentPropertySaveViewModel vm)
    {
        var imageCount = vm.Images?.Count ?? 0;
        if (imageCount < 1)
        {
            ModelState.AddModelError("Images", "Debe cargarse al menos una imagen de la propiedad.");
        }
        else if (imageCount > 4)
        {
            ModelState.AddModelError("Images", "No se deben permitir más de 4 imágenes por propiedad.");
        }

        var propertyTypes = await _propertyTypeService.GetAllViewModel();
        var saleTypes = await _saleTypeService.GetAllViewModel();
        
        if (!propertyTypes.Any(pt => pt.Id == vm.PropertyTypeId))
        {
            ModelState.AddModelError("PropertyTypeId", "El tipo de propiedad seleccionado no existe.");
        }
        
        if (!saleTypes.Any(st => st.Id == vm.SaleTypeId))
        {
            ModelState.AddModelError("SaleTypeId", "El tipo de venta seleccionado no existe.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.PropertyTypes = propertyTypes;
            ViewBag.SaleTypes = saleTypes;
            ViewBag.Improvements = await _improvementService.GetAllViewModel();
            return View(vm);
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        await _propertyService.CreateAsync(vm, userId);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var property = await _propertyService.GetPropertyDetailAsync(id);
        if (property != null && property.Status == PropertyStatus.Sold.ToString())
        {
            TempData["Error"] = "No puede editar una propiedad que ya ha sido vendida.";
            return RedirectToAction("Index");
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var vm = await _propertyService.GetPropertyForEditAsync(id, userId);
        if (vm is null) return NotFound();
        ViewBag.PropertyTypes = await _propertyTypeService.GetAllViewModel();
        ViewBag.SaleTypes = await _saleTypeService.GetAllViewModel();
        ViewBag.Improvements = await _improvementService.GetAllViewModel();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AgentPropertySaveViewModel vm)
    {
        var property = await _propertyService.GetPropertyDetailAsync(vm.Id);
        if (property != null && property.Status == PropertyStatus.Sold.ToString())
        {
            TempData["Error"] = "No puede editar una propiedad que ya ha sido vendida.";
            return RedirectToAction("Index");
        }

        var imageCount = (vm.Images?.Count ?? 0) + (vm.ExistingImages?.Count ?? 0) - (vm.ImagesToDelete?.Count ?? 0);
        if (imageCount < 1)
        {
            ModelState.AddModelError("Images", "Debe cargarse al menos una imagen de la propiedad.");
        }
        else if (imageCount > 4)
        {
            ModelState.AddModelError("Images", "No se deben permitir más de 4 imágenes por propiedad.");
        }

        var propertyTypes = await _propertyTypeService.GetAllViewModel();
        var saleTypes = await _saleTypeService.GetAllViewModel();
        
        if (!propertyTypes.Any(pt => pt.Id == vm.PropertyTypeId))
        {
            ModelState.AddModelError("PropertyTypeId", "El tipo de propiedad seleccionado no existe.");
        }
        
        if (!saleTypes.Any(st => st.Id == vm.SaleTypeId))
        {
            ModelState.AddModelError("SaleTypeId", "El tipo de venta seleccionado no existe.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.PropertyTypes = propertyTypes;
            ViewBag.SaleTypes = saleTypes;
            ViewBag.Improvements = await _improvementService.GetAllViewModel();
            return View(vm);
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        await _propertyService.UpdateAsync(vm, userId);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {
        var property = await _propertyService.GetPropertyDetailAsync(id);
        if (property != null && property.Status == RealEstateApp.Core.Domain.Enums.PropertyStatus.Sold.ToString())
        {
            TempData["Error"] = "No puede eliminar una propiedad que ya ha sido vendida.";
            return RedirectToAction("Index");
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var vm = await _propertyService.GetPropertyForEditAsync(id, userId);
        if (vm is null) return NotFound();
        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var property = await _propertyService.GetPropertyDetailAsync(id);
        if (property != null && property.Status == PropertyStatus.Sold.ToString())
        {
            TempData["Error"] = "No puede eliminar una propiedad que ya ha sido vendida.";
            return RedirectToAction("Index");
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        await _propertyService.DeleteAsync(id, userId);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Detail(int id)
    {
        var property = await _propertyService.GetPropertyDetailAsync(id);
        if (property == null) return NotFound();
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        if (property.AgentId != userId) return Forbid();
        
        ViewBag.Conversations = await _chatService.GetConversationsByPropertyAsync(id, userId);
        ViewBag.Offers = await _offerService.GetOfferSummaryByPropertyAsync(id, userId);
        
        return View(property);
    }

}

