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

        [HttpPost("GetAttendanceHistory")]
        
        public async Task<IActionResult> GetAttendanceHistory([FromBody] AttendanceHistoryRequest request)
        {
            // Retrieve PtslId, RoleId, and TeamId from Bearer Token
            var ptslId = User.FindFirst("PtslId")?.Value;
            var roleId = User.FindFirst("RoleId")?.Value;
            var teamId = User.FindFirst("TeamId")?.Value;

            if (ptslId == null || roleId == null)
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid token", data = (object)null });
            }

            // Convert RoleId and TeamId to the appropriate data types
            int parsedRoleId = int.Parse(roleId);
            int? parsedTeamId = teamId != null ? int.Parse(teamId) : (int?)null;

            // Set null for teamId if role is not 4, otherwise assign PtslId for RoleId 7
            if (parsedRoleId != 4)
            {
                parsedTeamId = null;
            }

            if (parsedRoleId == 7)
            {
                request.PtslId = ptslId;
            }

            // Prepare parameters for stored procedure
            var parameters = new[]
            {
                new SqlParameter("@PtslId", (object)request.PtslId ?? DBNull.Value),
                new SqlParameter("@TeamId", (object)parsedTeamId ?? DBNull.Value),
                new SqlParameter("@RoleId", parsedRoleId),
                new SqlParameter("@Date", (object)request.Date ?? DBNull.Value),
                new SqlParameter("@Month", (object)request.Month ?? DBNull.Value),
                new SqlParameter("@Year", (object)request.Year ?? DBNull.Value)
            };

            // Execute the stored procedure
            var attendanceHistory = await _context.AttendanceHistory
                .FromSqlRaw("EXEC [dbo].[GetAttendanceHistory] @PtslId, @TeamId, @RoleId, @Date, @Month, @Year", parameters)
                .ToListAsync();

            return Ok(new
            {
                statusCode = 200,
                message = "Attendance history fetched successfully",
                data = attendanceHistory
            });
        }
    }

    public class AttendanceHistoryRequest
    {
        public string? PtslId { get; set; }
        public DateTime? Date { get; set; } // New Date parameter
        public int? Month { get; set; }
        public int? Year { get; set; }
    }
}
