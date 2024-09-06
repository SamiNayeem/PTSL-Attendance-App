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
            // Retrieve the user's PtslId from the JWT token
            var ptslId = User.FindFirst("PtslId")?.Value;

            if (ptslId == null)
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid token" });
            }

            // Get the user entity
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PtslId == ptslId);

            if (user == null)
            {
                return Unauthorized(new { statusCode = 401, message = "User not found" });
            }

            // Fetch the office data based on the user's OfficeId
            var office = await _context.Offices
                .FirstOrDefaultAsync(o => o.IsActive);

            if (office == null)
            {
                return BadRequest(new { statusCode = 400, message = "Office not found or inactive" });
            }

            // Calculate the distance between the user's current position and office
            double distance = CalculateDistance(office.Latitude, office.Logitude, request.Latitude, request.Longitude);

            if (distance > office.Radius)
            {
                return BadRequest(new { statusCode = 400, message = "You are outside the allowed radius" });
            }

            // Record the attendance
            var attendance = new Attendance
            {
                UserId = ptslId,
                Users = user,  // Set the Users navigation property
                Date = DateTime.Now.Date,
                CheckIn = DateTime.Now,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                IsActive = true,
                CheckOut = DateTime.Now // Optional: Set CheckOut to current time, or you can set it later
            };

            _context.Attendance.Add(attendance);
            await _context.SaveChangesAsync();

            return Ok(new { statusCode = 200, message = "Attendance recorded successfully" });
        }

        // Haversine formula to calculate distance between two lat/long points
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371e3; // Earth radius in meters
            var latRad1 = lat1 * (Math.PI / 180); // Convert latitude from degrees to radians
            var latRad2 = lat2 * (Math.PI / 180);
            var deltaLat = (lat2 - lat1) * (Math.PI / 180);
            var deltaLon = (lon2 - lon1) * (Math.PI / 180);

            var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                    Math.Cos(latRad1) * Math.Cos(latRad2) *
                    Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var distance = R * c; // Distance in meters
            return distance;
        }
    }

    public class CheckInRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
