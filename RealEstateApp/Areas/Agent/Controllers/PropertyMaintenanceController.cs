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

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var propiedades = await _propiedadService.GetPropertiesByAgentAsync(userId);
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
    public async Task<IActionResult> Create(AgentPropertySaveViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.PropertyTypes = await _propertyTypeService.GetAllViewModel();
            ViewBag.SaleTypes = await _saleTypeService.GetAllViewModel();
            ViewBag.Improvements = await _improvementService.GetAllViewModel();
            return View(vm);
        }
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        await _propiedadService.CreateAsync(vm, userId);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var property = await _propiedadService.GetPropertyDetailAsync(id);
        if (property != null && property.Status == PropertyStatus.Sold.ToString())
        {
            TempData["Error"] = "No puede editar una propiedad que ya ha sido vendida.";
            return RedirectToAction("Index");
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var vm = await _propiedadService.GetPropertyForEditAsync(id, userId);
        if (vm is null) return NotFound();
        ViewBag.PropertyTypes = await _propertyTypeService.GetAllViewModel();
        ViewBag.SaleTypes = await _saleTypeService.GetAllViewModel();
        ViewBag.Improvements = await _improvementService.GetAllViewModel();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(AgentPropertySaveViewModel vm)
    {
        var property = await _propiedadService.GetPropertyDetailAsync(vm.Id);
        if (property != null && property.Status == PropertyStatus.Sold.ToString())
        {
            TempData["Error"] = "No puede editar una propiedad que ya ha sido vendida.";
            return RedirectToAction("Index");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.PropertyTypes = await _propertyTypeService.GetAllViewModel();
            ViewBag.SaleTypes = await _saleTypeService.GetAllViewModel();
            ViewBag.Improvements = await _improvementService.GetAllViewModel();
            return View(vm);
        }
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        await _propiedadService.UpdateAsync(vm, userId);
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
        var vm = await _propiedadService.GetPropertyForEditAsync(id, userId);
        if (vm is null) return NotFound();
        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var property = await _propiedadService.GetPropertyDetailAsync(id);
        if (property != null && property.Status == PropertyStatus.Sold.ToString())
        {
            TempData["Error"] = "No puede eliminar una propiedad que ya ha sido vendida.";
            return RedirectToAction("Index");
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        await _propiedadService.DeleteAsync(id, userId);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Detail(int id)
    {
        var property = await _propertyService.GetPropertyDetailAsync(id);
        if (property == null) return NotFound();
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        
        ViewBag.Conversations = await _chatService.GetConversationsByPropertyAsync(id, userId);
        ViewBag.Offers = await _offerService.GetOfferSummaryByPropertyAsync(id, userId);
        
        return View(property);
    }

}

