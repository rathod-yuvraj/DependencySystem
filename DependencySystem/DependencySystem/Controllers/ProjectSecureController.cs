using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DependencySystem.Controllers
{
    [Route("api/project/{projectId}/secure")]
    [ApiController]
    public class ProjectSecureController : ControllerBase
    {
        // ✅ Only Project Manager can access
        [HttpGet("manager")]
        [Authorize(Policy = "ProjectManagerOnly")]
        public IActionResult ManagerOnly(int projectId)
        {
            return Ok($"✅ Project {projectId} Manager Access granted");
        }

        // ✅ Any member can access
        [HttpGet("team")]
        [Authorize(Policy = "ProjectAnyMember")]
        public IActionResult AnyTeam(int projectId)
        {
            return Ok($"✅ Project {projectId} Team Member Access granted");
        }
    }
}
