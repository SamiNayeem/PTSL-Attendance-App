using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PTSLAttendanceManager.Data;
using PTSLAttendanceManager.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PTSLAttendanceManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ConfigController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("GetUserConfig")]
        public async Task<IActionResult> GetUserConfig()
        {
            // Retrieve the PtslId from the JWT token claims
            var PtslId = User.FindFirst("PtslId")?.Value;

            if (PtslId == null)
            {
                return Unauthorized(new
                {
                    statusCode = 401,
                    message = "Invalid token",
                    data = new object() { }
                });
            }

            var ptslIdParam = new SqlParameter("@PtslId", PtslId);

            // Call the stored procedure
            var result = await _context.UserConfigDtos
                .FromSqlRaw("EXEC Config @PtslId", ptslIdParam)
                .ToListAsync();

            if (result == null || result.Count == 0)
            {
                return NotFound(new
                {
                    statusCode = 404,
                    message = "User not found",
                    data = new object() { }
                });
            }

            return Ok(new
            {
                statusCode = 200,
                message = "Data retrieved successfully",
                data = result
            });
        }
    }
}
