using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PTSLAttendanceManager.Data;
using PTSLAttendanceManager.Models;
using System;
using System.Linq;
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

        [HttpGet("GetAttendance")]  // Change to POST as the body is passed
        [Authorize]  // Authorization via Bearer Token
        public async Task<IActionResult> GetAttendance([FromBody] AttendanceHistoryRequest request)
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

            long roleId = user.RoleId;
            

            

            // Execute stored procedure using FromSqlRaw with parameterized query
            var attendanceHistory = await _context.Database.SqlQueryRaw<AttendanceHistoryDto>("EXEC dbo.GetRoleBasedAttendance @PtslId, @RoleId, @Month, @Year",
                    new SqlParameter("@PtslId", ptslId),
                    new SqlParameter("@RoleId", roleId),
                    new SqlParameter("@Month", (object)request.Month ?? DBNull.Value),
                    new SqlParameter("@Year", (object)request.Year ?? DBNull.Value))
                                .ToListAsync();

            
            // Convert byte[] image to Base64 string for each attendance record


            foreach (var attendance in attendanceHistory)
                {
                    if (attendance.Image != null)
                    {
                        attendance.Image = Convert.FromBase64String(Convert.ToBase64String(attendance.Image));
                    }
                }


            return Ok(new
            {
                statusCode = 200,
                message = "Attendance data retrieved successfully",
                data = attendanceHistory
            });
        }
    }

    public class AttendanceHistoryRequest
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
    }
}
