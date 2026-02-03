using DependencySystem.DTOs.Team;
using DependencySystem.Services.Teams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DependencySystem.Controllers
{
    [Route("api/team")]
    [ApiController]
    [Authorize]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        // ✅ My Profile
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var profile = await _teamService.GetMyProfileAsync(userId);
            return Ok(profile);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile(UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var profile = await _teamService.UpdateMyProfileAsync(userId, dto);
            return Ok(profile);
        }

        // ✅ Assign user to project (Admin/Manager only)
        [HttpPost("assign")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AssignToProject(AssignProjectMemberDto dto)
        {
            var result = await _teamService.AssignUserToProjectAsync(dto);
            return Ok(result);
        }

        // ✅ Get project team list
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetProjectTeam(int projectId)
        {
            var team = await _teamService.GetProjectTeamAsync(projectId);
            return Ok(team);
        }

        // ✅ Remove member from project (Admin/Manager only)
        [HttpDelete("project/{projectId}/user/{userId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RemoveMember(int projectId, string userId)
        {
            var removed = await _teamService.RemoveProjectMemberAsync(projectId, userId);
            return removed ? Ok("Removed") : NotFound();
        }
    }
}
