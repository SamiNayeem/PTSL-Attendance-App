using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTSLAttendanceManager.Data;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PTSLAttendanceManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SelfAttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SelfAttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("MyAttendance")]
        public async Task<IActionResult> GetMyAttendanceHistory([FromBody] AttendanceFilterRequest request)
        {
            // Retrieve PtslId from the bearer token
            var ptslId = User.FindFirst("PtslId")?.Value;

            if (ptslId == null)
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid token", data = (object)null });
            }

            // Query the attendance records for the logged-in user
            var attendanceRecords = _context.Attendance
                .Include(a => a.Users)
                .Where(a => a.UserId == ptslId);

            // Apply filters if provided in the request
            if (request.Date != null)
            {
                attendanceRecords = attendanceRecords.Where(a => a.Date.Date == request.Date.Value.Date);
            }
            if (request.Month != null)
            {
                attendanceRecords = attendanceRecords.Where(a => a.Date.Month == request.Month);
            }
            if (request.Year != null)
            {
                attendanceRecords = attendanceRecords.Where(a => a.Date.Year == request.Year);
            }

            // Fetch results
            var result = await attendanceRecords
                .Select(a => new
                {
                    a.Date,
                    a.CheckIn,
                    a.CheckOut,
                    a.IsOnLocation,
                    a.Latitude,
                    a.Longitude
                })
                .ToListAsync();

            return Ok(new { statusCode = 200, message = "Attendance history retrieved", data = result });
        }
    }

    public class AttendanceFilterRequest
    {
        public DateTime? Date { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
    }
}
