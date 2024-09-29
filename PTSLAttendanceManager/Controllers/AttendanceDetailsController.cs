using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PTSLAttendanceManager.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PTSLAttendanceManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceDetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AttendanceDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("GetAttendanceDetails")]
        [Authorize]  // Authorization via Bearer Token
        public async Task<IActionResult> GetAttendanceDetails([FromBody] AttendanceDetailsRequest request)
        {
            if (string.IsNullOrEmpty(request.PtslId) || request.Date == default)
            {
                return BadRequest(new { statusCode = 400, message = "Invalid input. Please provide valid PTSLId and Date.", data = (object)null });
            }

            try
            {
                var attendanceDetails = await _context.Database.SqlQueryRaw<AttendanceDetailsDto>(
                        "EXEC dbo.GetUserAttendanceDetails @PtslId, @Date",
                        new SqlParameter("@PtslId", request.PtslId),
                        new SqlParameter("@Date", request.Date))
                    .ToListAsync();

                if (!attendanceDetails.Any())
                {
                    return NotFound(new { statusCode = 404, message = "No attendance data found for the specified date and user.", data = (object)null });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Attendance details retrieved successfully",
                    data = attendanceDetails
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving attendance details.",
                    error = ex.Message
                });
            }
        }
    }

    // DTO to match the result set of the stored procedure
    public class AttendanceDetailsDto
    {
        public string PtslId { get; set; }
        public DateTime Date { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public bool IsOnLocation { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    // Request DTO
    public class AttendanceDetailsRequest
    {
        public string? PtslId { get; set; }  // The PTSLId of the user for whom attendance details are required
        public DateTime? Date { get; set; }  // The specific date for which attendance details are required
    }
}
