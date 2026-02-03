using DependencySystem.DTOs.Task;
using DependencySystem.Services.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DependencySystem.Controllers
{
    [Route("api/task")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _taskService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var task = await _taskService.GetByIdAsync(id);
            return task == null ? NotFound() : Ok(task);
        }

        [HttpGet("module/{moduleId}")]
        public async Task<IActionResult> GetByModule(int moduleId)
            => Ok(await _taskService.GetByModuleIdAsync(moduleId));

        [HttpPost]
        public async Task<IActionResult> Create(TaskCreateDto dto)
            => Ok(await _taskService.CreateAsync(dto));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _taskService.DeleteAsync(id);
            return deleted ? Ok("Deleted") : NotFound();
        }
    }
}
