using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PTSLAttendanceManager.Data;
using PTSLAttendanceManager.Models;
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
        public async Task<IActionResult> GetUserConfig([FromBody] PtslIdRequest body)
        {
            // Extract PtslId from the JSON request body
            string PtslId = body.PtslId;

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
                    data = new List<object>()
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
