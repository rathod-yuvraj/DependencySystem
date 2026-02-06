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
        private readonly IRoleDashboardService _roledashboardService;
        public DashboardController(IDashboardService dashboardService, IRoleDashboardService roledashboardService) {


            _dashboardService = dashboardService;
            _roledashboardService = roledashboardService;
        }
    
        //[Authorize(Roles = "Manager")]
        [HttpGet("manager/project/{projectId}")]
        public async Task<IActionResult> ManagerDashboard(int projectId)
        {
            return Ok(await _roledashboardService.GetManagerDashboardAsync(projectId));
        }
        [HttpGet("project/{projectId}/summary")]
        public async Task<IActionResult> GetProjectDashboard(int projectId)
        {
            return Ok(await _dashboardService.GetProjectDashboardAsync(projectId));
        }


        //[Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            return Ok(await _roledashboardService.GetAdminDashboardAsync());
        }

       

        //[Authorize(Roles = "Developer")]
        [HttpGet("developer")]
        public async Task<IActionResult> DeveloperDashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return Ok(await _roledashboardService.GetDeveloperDashboardAsync(userId));
        }
      

        [HttpGet("maintainer")]
        //[Authorize(Roles = AppRoles.Maintainer)]
        public IActionResult MaintainerDashboard()
            => Ok("✅ Welcome Maintainer Dashboard");
    }

}
