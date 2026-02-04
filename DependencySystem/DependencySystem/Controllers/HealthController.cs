using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DependencySystem.Controllers
{
    [Route("api/health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public IActionResult Check()
            => Ok(" DependencySystem API Running");
    }
}
