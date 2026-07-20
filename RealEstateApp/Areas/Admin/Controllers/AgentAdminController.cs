using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class AgentAdminController(IUserManagementService service) : Controller
    {
        private readonly IUserManagementService _service = service;

        public async Task<IActionResult> Index()
        {
            var agents = await _service.GetAllAgents();
            return View(agents);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id, bool activate)
        {
            var result = await _service.ToggleAgentStatus(id, activate);
            TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
                ? (activate ? "El agente fue activado correctamente." : "El agente fue inactivado correctamente.")
                : string.Join(" ", result.Errors);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            var agents = await _service.GetAllAgents();
            var agent = agents.FirstOrDefault(a => a.Id == id);
            if (agent is null) return NotFound();
            return View(agent);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var result = await _service.DeleteAgent(id);
            TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
                ? "El agente fue eliminado correctamente." : string.Join(" ", result.Errors);
            return RedirectToAction(nameof(Index));
        }
    }
}