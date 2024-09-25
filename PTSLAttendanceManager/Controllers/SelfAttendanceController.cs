//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore;
//using PTSLAttendanceManager.Data;
//using PTSLAttendanceManager.Models;
//using System.Linq;
//using System.Threading.Tasks;

//namespace PTSLAttendanceManager.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class SelfAttendanceController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;

//        public SelfAttendanceController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet("MyAttendance")]  // Use POST since we are passing Month and Year in the body
//        [Authorize]
//        public async Task<IActionResult> GetMyAttendanceHistory([FromBody] AttendanceFilterRequest request)
//        {
//            // Retrieve PtslId from the bearer token
//            var ptslId = User.FindFirst("PtslId")?.Value;

//            if (string.IsNullOrEmpty(ptslId))
//            {
//                return Unauthorized(new { statusCode = 401, message = "Invalid token", data = (object)null });
//            }

//            // Execute the stored procedure with parameters
//            var attendanceRecords = await _context.Set<AttendanceHistoryDto>()
//                .FromSqlRaw("EXEC EmployeeAttendanceHistory @PtslId, @Month, @Year",
//                    new SqlParameter("@PtslId", ptslId),
//                    new SqlParameter("@Month", (object)request.Month ?? DBNull.Value),
//                    new SqlParameter("@Year", (object)request.Year ?? DBNull.Value))
//                .ToListAsync();

//            if (attendanceRecords == null || attendanceRecords.Count == 0)
//            {
//                return NotFound(new { statusCode = 404, message = "No attendance records found", data = (object)null });
//            }

//            // Project the entire list of attendance records to the desired format
//            var result = attendanceRecords.Select(record => new
//            {
//                PtslId = record.PtslId,
//                Name = record.Name,
//                TeamName = record.TeamName,
//                Date = record.Date,
//                CheckIn = record.CheckIn,
//                CheckOut = record.CheckOut,
//                IsOnLocation = record.IsOnLocation,
//                Latitude = record.AttendanceLatitude,
//                Longitude = record.AttendanceLongitude,
//                Title = record.Title,
//                Description = record.Description,
//                //Image = record.Image != null ? Convert.ToBase64String(record.Image) : null
//            }).ToList();

//            return Ok(new
//            {
//                statusCode = 200,
//                message = "Attendance history retrieved",
//                data = result
//            });
//        }
//    }

//    public class AttendanceFilterRequest
//    {
//        public int? Month { get; set; }
//        public int? Year { get; set; }
//    }
//}

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

        [HttpGet("MyAttendance")]
        public async Task<IActionResult> GetMyAttendanceHistory([FromQuery] AttendanceFilterRequest request)
        {
            // Retrieve PtslId from the bearer token
            var ptslId = User.FindFirst("PtslId")?.Value;

            if (string.IsNullOrEmpty(ptslId))
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid token", data = (object)null });
            }

            // Initialize query to retrieve attendance records
            var attendanceRecords = _context.Attendance
                .Where(a => a.UserId == ptslId);  // Filter by PtslId

            // Apply filters based on request
            if (request.Date.HasValue)
            {
                attendanceRecords = attendanceRecords.Where(a => a.Date.Date == request.Date.Value.Date);
            }
            if (request.Month.HasValue)
            {
                attendanceRecords = attendanceRecords.Where(a => a.Date.Month == request.Month);
            }
            if (request.Year.HasValue)
            {
                attendanceRecords = attendanceRecords.Where(a => a.Date.Year == request.Year);
            }

            // Fetch results and project them to a DTO or anonymous object
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