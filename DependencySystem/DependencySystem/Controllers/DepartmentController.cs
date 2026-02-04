using DependencySystem.DTOs.Department;
using DependencySystem.Services.Departments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DependencySystem.Controllers
{
    [Route("api/department")]
    [ApiController]
    //[Authorize]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _departmentService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dept = await _departmentService.GetByIdAsync(id);
            return dept == null ? NotFound() : Ok(dept);
        }

        [HttpGet("company/{companyId}")]
        public async Task<IActionResult> GetByCompany(int companyId)
            => Ok(await _departmentService.GetByCompanyIdAsync(companyId));

        [HttpPost]
        public async Task<IActionResult> Create(DepartmentCreateDto dto)
        {
            var dept = await _departmentService.CreateAsync(dto);
            return Ok(dept);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _departmentService.DeleteAsync(id);
            return deleted ? Ok("Deleted") : NotFound();
        }
    }
}
