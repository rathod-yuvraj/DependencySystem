using DependencySystem.DTOs.Dependency;
using DependencySystem.Services.Dependencies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DependencySystem.Controllers
{
    [Route("api/dependency")]
    [ApiController]
    [Authorize]
    public class DependencyController : ControllerBase
    {
        private readonly IDependencyService _service;

        public DependencyController(IDependencyService service)
        {
            _service = service;
        }

        // ===================== MODULE DEPENDENCY =====================

        [HttpGet("module/{moduleId}")]
        public async Task<IActionResult> GetModuleDependencies(int moduleId)
        {
            var data = await _service.GetModuleDependenciesAsync(moduleId);
            return Ok(data);
        }

        [HttpPost("module")]
        public async Task<IActionResult> AddModuleDependency(AddModuleDependencyDto dto)
        {
            var result = await _service.AddModuleDependencyAsync(dto);
            return Ok(result);
        }

        [HttpDelete("module/{dependencyId}")]
        public async Task<IActionResult> RemoveModuleDependency(int dependencyId)
        {
            var deleted = await _service.RemoveModuleDependencyAsync(dependencyId);
            return deleted ? Ok("Deleted") : NotFound();
        }

        // ===================== TASK DEPENDENCY =====================

        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetTaskDependencies(int taskId)
        {
            var data = await _service.GetTaskDependenciesAsync(taskId);
            return Ok(data);
        }

        [HttpPost("task")]
        public async Task<IActionResult> AddTaskDependency(AddTaskDependencyDto dto)
        {
            var result = await _service.AddTaskDependencyAsync(dto);
            return Ok(result);
        }

        [HttpDelete("task/{taskId}/{dependsOnTaskId}")]
        public async Task<IActionResult> RemoveTaskDependency(int taskId, int dependsOnTaskId)
        {
            var deleted = await _service.RemoveTaskDependencyAsync(taskId, dependsOnTaskId);
            return deleted ? Ok("Deleted") : NotFound();
        }
    }
}
