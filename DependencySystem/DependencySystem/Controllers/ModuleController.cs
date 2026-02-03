using DependencySystem.DTOs.Module;
using DependencySystem.Services.Modules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DependencySystem.Controllers
{
    [Route("api/module")]
    [ApiController]
    [Authorize]
    public class ModuleController : ControllerBase
    {
        private readonly IModuleService _moduleService;

        public ModuleController(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _moduleService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var module = await _moduleService.GetByIdAsync(id);
            return module == null ? NotFound() : Ok(module);
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetByProject(int projectId)
            => Ok(await _moduleService.GetByProjectIdAsync(projectId));

        [HttpPost]
        public async Task<IActionResult> Create(ModuleCreateDto dto)
            => Ok(await _moduleService.CreateAsync(dto));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _moduleService.DeleteAsync(id);
            return deleted ? Ok("Deleted") : NotFound();
        }
    }
}
