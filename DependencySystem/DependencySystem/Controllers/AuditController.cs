//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace DependencySystem.Controllers
//{
//    public class AuditController : Controller
//    {
//        [HttpGet("project/{projectId}")]
//        public async Task<IActionResult> GetProjectAudit(int projectId)
//        {
//            var logs = await _context.AuditLogs
//                .Where(x => x.ProjectId == projectId)
//                .OrderByDescending(x => x.CreatedAt)
//                .Take(50)
//                .ToListAsync();

//            return Ok(logs);
//        }

//    }
//}
