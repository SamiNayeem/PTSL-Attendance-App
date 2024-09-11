using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PTSLAttendanceManager.Data;

namespace PTSLAttendanceManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceConfigController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AttendanceConfigController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Config")]
        [Authorize]
        public async Task<IActionResult> GetProfileConfig()
        {
            var ptslId = User.FindFirst("PtslId")?.Value;

            if (ptslId == null)
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid token", data = (object)null });
            }

            // Fetch data from the stored procedure and bind the parameter
            var attendanceConfig = await _context.AttendanceConfigResult
                .FromSqlRaw("EXEC AttendanceConfig @PtslId = {0}", ptslId)
                .ToListAsync();

            if (!attendanceConfig.Any())
            {
                return NotFound(new { statusCode = 404, message = "No attendance data found", data = (object)null });
            }

            var result = attendanceConfig.First();

            return Ok(new
            {
                statusCode = 200,
                message = "Attendance config retrieved successfully",
                data = new
                {
                    OfficeLat = result.OfficeLatitude,
                    OfficeLong = result.OfficeLongitude,
                    OfficeRadius = result.OfficeRadius,
                    Date = DateTime.Now.Date,
                    CheckIn = result.CheckInTime,
                    CheckOut = result.CheckOutTime
                }
            });
        }

        public class AttendanceConfigResult
        {
            public double OfficeLatitude { get; set; }
            public double OfficeLongitude { get; set; }
            public double OfficeRadius { get; set; }
            public DateTime? AttendanceDate { get; set; }
            public DateTime? CheckInTime { get; set; }
            public DateTime? CheckOutTime { get; set; }
        }
    }
}
