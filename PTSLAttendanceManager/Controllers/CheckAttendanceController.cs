using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PTSLAttendanceManager.Data;
using PTSLAttendanceManager.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PTSLAttendanceManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckAttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CheckAttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAttendance")]
        [Authorize] // Ensure the user is authenticated
        public async Task<IActionResult> GetAttendance([FromQuery] AttendanceHistoryRequest request)
        {
            // Retrieve PtslId from the JWT token claims
            var ptslId = User.FindFirst("PtslId")?.Value;

            if (string.IsNullOrEmpty(ptslId))
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid token or role", data = (object)null });
            }

            // Retrieve the user's RoleId and TeamId from the database
            var user = await _context.Users
                .Where(u => u.PtslId == ptslId)
                .Select(u => new { u.RoleId, u.TeamId })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid token or role", data = (object)null });
            }

            // Handle nullable types with a default value or explicit conversion
            long roleId = user.RoleId ; // Use a default value if RoleId is null
            long teamId = user.TeamId ?? 0; // Use a default value if TeamId is null

            if (teamId == 0)
            {
                return NotFound(new { statusCode = 404, message = "User's team not found", data = (object)null });
            }

            // Execute stored procedure using FromSqlRaw with parameterized query
            var result = await _context.Set<AttendanceHistoryDto>()
    .FromSqlRaw("EXEC dbo.GetRoleBasedAttendance @PtslId, @RoleId, @Month, @Year",
                new SqlParameter("@PtslId", ptslId),
                new SqlParameter("@RoleId", roleId),
                new SqlParameter("@Month", (object)request.Month ?? DBNull.Value),
                new SqlParameter("@Year", (object)request.Year ?? DBNull.Value))
    .ToListAsync();



            return Ok(new
            {
                statusCode = 200,
                message = "Attendance data retrieved successfully",
                data = result
            });
        }
    }

    public class AttendanceHistoryRequest
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
    }
}
