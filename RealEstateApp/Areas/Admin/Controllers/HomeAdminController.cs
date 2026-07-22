using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class HomeAdminController(IUserManagementService userManagementService) : Controller
    {
        private readonly IUserManagementService _userManagementService = userManagementService;

        public async Task<IActionResult> GetDashboardModal()
        {
            var dashboard = await _userManagementService.GetDashboardCounts();
            return PartialView("_DashboardModal", dashboard);
        }
    }
}
