using DependencySystem.Helper;
using DependencySystem.Services.Companies;
using DependencySystem.Services.Dashboard;


//using DependencySystem.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DependencySystem.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService) {


            _dashboardService = _dashboardService;
        }

        [HttpGet("project/{projectId}/summary")]
        public async Task<IActionResult> GetProjectDashboard(int projectId)
        {
            return Ok(await _dashboardService.GetProjectDashboardAsync(projectId));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            return Ok(await _dashboardService.GetAdminDashboardAsync());
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("manager/project/{projectId}")]
        public async Task<IActionResult> ManagerDashboard(int projectId)
        {
            return Ok(await _dashboardService.GetManagerDashboardAsync(projectId));
        }

        [Authorize(Roles = "Developer")]
        [HttpGet("developer")]
        public async Task<IActionResult> DeveloperDashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return Ok(await _dashboardService.GetDeveloperDashboardAsync(userId));
        }

        [HttpGet("maintainer")]
        //[Authorize(Roles = AppRoles.Maintainer)]
        public IActionResult MaintainerDashboard()
            => Ok("✅ Welcome Maintainer Dashboard");
    }
}
