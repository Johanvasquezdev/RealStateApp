using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.SaleType;

namespace RealEstateApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class SaleTypesController(ISaleTypeService service) : Controller
    {
        private readonly ISaleTypeService _service = service;

        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllViewModel();
            return View(list);
        }

        public IActionResult Create() => View(new SaveSaleTypeViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveSaleTypeViewModel vm)
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

            TempData["Success"] = "El tipo de venta fue creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.GetByIdViewModel(id);
            var saveVm = new SaveSaleTypeViewModel { Id = vm.Id, Name = vm.Name, Description = vm.Description };
            return View(saveVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SaveSaleTypeViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                await _service.Update(vm, vm.Id);
                TempData["Success"] = "El tipo de venta fue actualizado correctamente.";
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
                TempData["Success"] = "El tipo de venta fue eliminado correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}