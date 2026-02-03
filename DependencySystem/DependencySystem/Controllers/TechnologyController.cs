using DependencySystem.DTOs.Technology;
using DependencySystem.Services.Technologies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DependencySystem.Controllers
{
    [Route("api/technology")]
    [ApiController]
    [Authorize]
    public class TechnologyController : ControllerBase
    {
        private readonly ITechnologyService _service;

        public TechnologyController(ITechnologyService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(TechnologyCreateDto dto)
            => Ok(await _service.CreateAsync(dto));

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            return ok ? Ok("Deleted") : NotFound();
        }

        [HttpPost("project/{projectId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AssignToProject(int projectId, AssignTechnologyDto dto)
            => Ok(await _service.AssignToProjectAsync(projectId, dto.TechnologyID));

        [HttpPost("module/{moduleId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AssignToModule(int moduleId, AssignTechnologyDto dto)
            => Ok(await _service.AssignToModuleAsync(moduleId, dto.TechnologyID));

        [HttpPost("user/{userId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AssignToUser(string userId, AssignTechnologyDto dto)
            => Ok(await _service.AssignToUserAsync(userId, dto.TechnologyID));
    }
}
