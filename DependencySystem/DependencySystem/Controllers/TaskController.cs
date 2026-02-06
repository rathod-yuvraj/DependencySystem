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

        /* ================= GET ALL ================= */

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _taskService.GetAllAsync());

        /* ================= GET BY ID ================= */

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var task = await _taskService.GetByIdAsync(id);
            return task == null ? NotFound() : Ok(task);
        }

        /* ================= GET BY MODULE ================= */

        [HttpGet("module/{moduleId}")]
        public async Task<IActionResult> GetByModule(int moduleId)
            => Ok(await _taskService.GetByModuleIdAsync(moduleId));

        /* ================= CREATE ================= */

        [HttpPost]
        public async Task<IActionResult> Create(TaskCreateDto dto)
        {
            var created = await _taskService.CreateAsync(dto);
            return Ok(created);
        }

        /* ================= UPDATE ================= */

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TaskUpdateDto dto)
        {
            var updated = await _taskService.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        /* ================= DELETE ================= */

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _taskService.DeleteAsync(id);
            return deleted ? Ok("Deleted") : NotFound();
        }
    }
}
