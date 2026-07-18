using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Improvement;

namespace RealEstateApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class ImprovementsController(IImprovementService service) : Controller
    {
        private readonly IImprovementService _service = service;

        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllViewModel();
            return View(list);
        }

        public IActionResult Create() => View(new SaveImprovementViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveImprovementViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            await _service.Add(vm);
            TempData["Success"] = "La mejora fue creada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.GetByIdViewModel(id);
            var saveVm = new SaveImprovementViewModel { Id = vm.Id, Name = vm.Name, Description = vm.Description };
            return View(saveVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SaveImprovementViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            await _service.Update(vm, vm.Id);
            TempData["Success"] = "La mejora fue actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var vm = await _service.GetByIdViewModel(id);
            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.Delete(id);
            TempData["Success"] = "La mejora fue eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}