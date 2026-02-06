using DependencySystem.DTOs.Project;
using DependencySystem.Services.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DependencySystem.Controllers
{
    [Route("api/project")]
    [ApiController]
    //[Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _projectService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var project = await _projectService.GetByIdAsync(id);
            return project == null ? NotFound() : Ok(project);
        }
        [HttpGet("{projectId}/tree")]
        public async Task<IActionResult> GetProjectTree(int projectId)
        {
            var result = await _projectService.GetProjectTreeAsync(projectId);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpGet("department/{departmentId}")]
        public async Task<IActionResult> GetByDepartment(int departmentId)
            => Ok(await _projectService.GetByDepartmentIdAsync(departmentId));

        [HttpPost]
        public async Task<IActionResult> Create(ProjectCreateDto dto)
            => Ok(await _projectService.CreateAsync(dto));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _projectService.DeleteAsync(id);
            return deleted ? Ok("Deleted") : NotFound();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProjectUpdateDto dto)
        {
            var updated = await _projectService.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }


    }
}
