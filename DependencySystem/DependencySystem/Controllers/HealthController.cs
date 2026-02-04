using Microsoft.AspNetCore.Mvc;

namespace DependencySystem.Controllers
{
    [Route("api/health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Check()
            => Ok(" DependencySystem API Running");
    }
}
