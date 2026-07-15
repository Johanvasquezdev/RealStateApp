using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class HomeAdminController(IUserManagementService userManagementService) : Controller
    {
        private readonly IUserManagementService _userManagementService = userManagementService;

        public async Task<IActionResult> Index()
        {
            var dashboard = await _userManagementService.GetDashboardCounts();
            return View(dashboard);
        }
    }
}
