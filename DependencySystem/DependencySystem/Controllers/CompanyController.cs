using DependencySystem.DTOs.Company;
using DependencySystem.Services.Companies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DependencySystem.Controllers
{
    [Route("api/company")]
    [ApiController]
    [Authorize] // ✅ only logged-in users
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _companyService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var company = await _companyService.GetByIdAsync(id);
            return company == null ? NotFound() : Ok(company);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CompanyCreateDto dto)
        {
            var company = await _companyService.CreateAsync(dto);
            return Ok(company);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _companyService.DeleteAsync(id);
            return deleted ? Ok("Deleted") : NotFound();
        }
    }
}
