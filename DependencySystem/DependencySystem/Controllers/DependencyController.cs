using Microsoft.AspNetCore.Mvc;

namespace DependencySystem.Controllers
{
    public class DependencyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
