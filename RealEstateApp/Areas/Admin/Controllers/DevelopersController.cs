using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Users;

namespace RealEstateApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class DevelopersController(IUserManagementService service) : Controller
    {
        private readonly IUserManagementService _service = service;

        public async Task<IActionResult> Index()
        {
            var admins = await _service.GetAllDevelopers();
            return View(admins);
        }

        public IActionResult Create() => View(new SaveDeveloperViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveDeveloperViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var result = await _service.CreateDeveloper(vm);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e);
                return View(vm);
            }

            TempData["Success"] = "El desarrollador fue creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var admins = await _service.GetAllDevelopers();
            var admin = admins.FirstOrDefault(a => a.Id == id);
            if (admin is null) return NotFound();

            var vm = new SaveDeveloperViewModel
            {
                Id = admin.Id,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                IdCard = admin.IdCard ?? string.Empty,
                Email = admin.Email,
                UserName = admin.UserName
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SaveDeveloperViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var result = await _service.UpdateDeveloper(vm);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e);
                return View(vm);
            }

            TempData["Success"] = "El desarrollador fue actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id, bool activate)
        {
            var result = await _service.ToggleDeveloperStatus(id, activate);
            TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
                ? (activate ? "El desarrollador fue activado correctamente." : "El desarrollador fue inactivado correctamente.")
                : string.Join(" ", result.Errors);
            return RedirectToAction(nameof(Index));
        }
    }
}