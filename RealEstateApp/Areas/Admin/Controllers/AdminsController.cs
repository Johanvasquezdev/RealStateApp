using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Users;

namespace RealEstateApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class AdminsController(IUserManagementService service) : Controller
    {
        private readonly IUserManagementService _service = service;

        public async Task<IActionResult> Index()
        {
            var admins = await _service.GetAllAdmins();
            return View(admins);
        }

        public IActionResult Create() => View(new SaveAdminViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveAdminViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var result = await _service.CreateAdmin(vm);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e);
                return View(vm);
            }

            TempData["Success"] = "El administrador fue creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var admins = await _service.GetAllAdmins();
            var admin = admins.FirstOrDefault(a => a.Id == id);
            if (admin is null) return NotFound();

            var vm = new SaveAdminViewModel
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
        public async Task<IActionResult> Edit(SaveAdminViewModel vm)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (vm.Id == currentUserId)
            {
                ModelState.AddModelError(string.Empty, "No se puede editar su propio usuario desde este mantenimiento.");
                return View(vm);
            }

            if (!ModelState.IsValid) return View(vm);

            var result = await _service.UpdateAdmin(vm);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e);
                return View(vm);
            }

            TempData["Success"] = "El administrador fue actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id, bool activate)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var result = await _service.ToggleAdminStatus(id, activate, currentUserId);
            TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
                ? (activate ? "El administrador fue activado correctamente." : "El administrador fue inactivado correctamente.")
                : string.Join(" ", result.Errors);
            return RedirectToAction(nameof(Index));
        }
    }
}