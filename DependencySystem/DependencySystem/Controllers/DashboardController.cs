using Microsoft.AspNetCore.Mvc;

namespace DependencySystem.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
