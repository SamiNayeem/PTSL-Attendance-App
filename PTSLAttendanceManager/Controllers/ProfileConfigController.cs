using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Correct import for Authorize attribute
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
    public class ProfileConfigController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfileConfigController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetUserProfile")]
        [Authorize] // Ensure correct authorization
        public async Task<IActionResult> GetProfileConfig()
        {
            // Retrieve the PtslId from the JWT token claims
            var ptslId = User.FindFirst("PtslId")?.Value;

            if (string.IsNullOrEmpty(ptslId))
            {
                return Unauthorized(new
                {
                    statusCode = 401,
                    message = "Invalid token",
                    data = new object() { }
                });
            }

            var ptslIdParam = new SqlParameter("@PtslId", ptslId);

            try
            {
                // Execute stored procedure using FromSqlRaw with parameterized query
                var result = await _context.UserConfigDtos
                    .FromSqlRaw("EXEC Config @PtslId", ptslIdParam)
                    .ToListAsync();

                if (!result.Any())
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
            catch (Exception ex)
            {
                // Catch any exceptions and return an appropriate error response
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving data.",
                    error = ex.Message
                });
            }
        }
    }
}
