using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Property;

namespace RealEstateApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class PropertyTypesController(IPropertyTypeService service) : Controller
    {
        private readonly IPropertyTypeService _service = service;

        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllViewModel();
            return View(list);
        }

        public IActionResult Create() => View(new SavePropertyTypeViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SavePropertyTypeViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                await _service.Add(vm);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("Name", ex.Message);
                return View(vm);
            }

            TempData["Success"] = "El tipo de propiedad fue creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.GetByIdViewModel(id);
            var saveVm = new SavePropertyTypeViewModel { Id = vm.Id, Name = vm.Name, Description = vm.Description };
            return View(saveVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SavePropertyTypeViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                await _service.Update(vm, vm.Id);
                TempData["Success"] = "El tipo de propiedad fue actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("Name", ex.Message);
                return View(vm);
            }
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _service.Delete(id);
                TempData["Success"] = "El tipo de propiedad fue eliminado correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}