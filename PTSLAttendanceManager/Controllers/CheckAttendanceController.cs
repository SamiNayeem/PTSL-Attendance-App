using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PTSLAttendanceManager.Data;
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

        [HttpPost("GetAttendance")]
        [Authorize] // Ensure the user is authenticated
        public async Task<IActionResult> GetAttendance([FromBody] AttendanceHistoryRequest request)
        {
            var ptslId = User.FindFirst("PtslId")?.Value;
            var roleId = int.Parse(User.FindFirst(ClaimTypes.Role)?.Value ?? "0");

            if (ptslId == null || roleId == 0)
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid token", data = (object)null });
            }

            var teamId = await _context.Users
                .Where(u => u.PtslId == ptslId)
                .Select(u => u.TeamId)
                .FirstOrDefaultAsync();

            if (teamId == 0)
            {
                return Unauthorized(new { statusCode = 401, message = "User's team not found", data = (object)null });
            }

            var result = await _context.AttendanceHistory
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
