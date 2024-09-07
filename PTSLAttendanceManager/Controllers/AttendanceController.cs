using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTSLAttendanceManager.Data;
using PTSLAttendanceManager.Models.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PTSLAttendanceManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("CheckIn")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
        {
            var ptslId = User.FindFirst("PtslId")?.Value;

            if (ptslId == null)
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid token", data = (object)null });
            }

            var user = await _context.Users.Include(u => u.Offices) // Make sure to include the user's Office
                .FirstOrDefaultAsync(u => u.PtslId == ptslId);

            if (user == null)
            {
                return Unauthorized(new { statusCode = 401, message = "User not found", data = (object)null });
            }

            var office = user.Offices;  // Get the user's specific office

            if (office == null || !office.IsActive)
            {
                return BadRequest(new { statusCode = 400, message = "User's office not found or inactive", data = (object)null });
            }

            double distance = CalculateDistance(office.Latitude, office.Logitude, request.Latitude, request.Longitude);

            if (distance > office.Radius)
            {
                return BadRequest(new { statusCode = 400, message = "You are outside the allowed radius", data = (object)null });
            }

            var attendance = new Attendance
            {
                UserId = ptslId,
                Users = user,
                Date = DateTime.Now.Date,
                CheckIn = DateTime.Now,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                IsActive = true
            };

            _context.Attendance.Add(attendance);
            await _context.SaveChangesAsync();

            return Ok(new { statusCode = 200, message = "Attendance recorded successfully", data = new { attendance.Id, attendance.Date, attendance.CheckIn, attendance.Latitude, attendance.Longitude } });
        }



        [HttpPost("CheckOut")]
        public async Task<IActionResult> CheckOut([FromBody] CheckInRequest request)
        {
            var ptslId = User.FindFirst("PtslId")?.Value;

            if (ptslId == null)
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid token", data = (object)null });
            }

            var user = await _context.Users.Include(u => u.Offices) // Include the user's Office
                .FirstOrDefaultAsync(u => u.PtslId == ptslId);

            if (user == null)
            {
                return Unauthorized(new { statusCode = 401, message = "User not found", data = (object)null });
            }

            var office = user.Offices;  // Get the user's specific office

            if (office == null || !office.IsActive)
            {
                return BadRequest(new { statusCode = 400, message = "User's office not found or inactive", data = (object)null });
            }

            double distance = CalculateDistance(office.Latitude, office.Logitude, request.Latitude, request.Longitude);

            if (distance > office.Radius)
            {
                return BadRequest(new { statusCode = 400, message = "You are outside the allowed radius", data = (object)null });
            }

            var activeAttendance = await _context.Attendance
                .FirstOrDefaultAsync(a => a.UserId == ptslId && a.IsActive && a.CheckOut == null);

            if (activeAttendance == null)
            {
                return BadRequest(new { statusCode = 400, message = "No active attendance found for checkout", data = (object)null });
            }

            activeAttendance.CheckOut = DateTime.Now;
            activeAttendance.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                statusCode = 200,
                message = "Checked out successfully",
                data = new
                {
                    activeAttendance.Id,
                    activeAttendance.Date,
                    activeAttendance.CheckIn,
                    activeAttendance.CheckOut,
                    activeAttendance.Latitude,
                    activeAttendance.Longitude
                }
            });
        }



        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371e3; // Earth radius in meters
            var latRad1 = lat1 * (Math.PI / 180);
            var latRad2 = lat2 * (Math.PI / 180);
            var deltaLat = (lat2 - lat1) * (Math.PI / 180);
            var deltaLon = (lon2 - lon1) * (Math.PI / 180);

            var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                    Math.Cos(latRad1) * Math.Cos(latRad2) *
                    Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var distance = R * c;
            return distance;
        }
    }

    public class CheckInRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
