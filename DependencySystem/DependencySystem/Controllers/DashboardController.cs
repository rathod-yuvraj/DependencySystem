using DependencySystem.Helper;
//using DependencySystem.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DependencySystem.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        [HttpGet("admin")]
        [Authorize(Roles = AppRoles.Admin)]
        public IActionResult AdminDashboard()
            => Ok("✅ Welcome Admin Dashboard");

        [HttpGet("manager")]
        [Authorize(Roles = AppRoles.Manager)]
        public IActionResult ManagerDashboard()
            => Ok("✅ Welcome Manager Dashboard");

        [HttpGet("developer")]
        [Authorize(Roles = AppRoles.Developer)]
        public IActionResult DeveloperDashboard()
            => Ok("✅ Welcome Developer Dashboard");

        [HttpGet("maintainer")]
        [Authorize(Roles = AppRoles.Maintainer)]
        public IActionResult MaintainerDashboard()
            => Ok("✅ Welcome Maintainer Dashboard");
    }
}
