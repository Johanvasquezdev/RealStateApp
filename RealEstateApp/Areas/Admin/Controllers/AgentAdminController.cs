using Microsoft.AspNetCore.Mvc;

namespace RealEstateApp.Areas.Admin.Controllers
{
    public class AgentAdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
