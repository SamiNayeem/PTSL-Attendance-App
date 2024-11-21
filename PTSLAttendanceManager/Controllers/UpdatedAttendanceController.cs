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
    public class UpdatedAttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UpdatedAttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Attendance")]
        public async Task<IActionResult> HandleAttendance([FromBody] AttendanceRequest request)
        {
            var ptslId = User.FindFirst("PtslId")?.Value;

            if (ptslId == null)
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid token", data = (object)null });
            }

            var user = await _context.Users.Include(u => u.Offices)
                .FirstOrDefaultAsync(u => u.PtslId == ptslId);

            if (user == null)
            {
                return Unauthorized(new { statusCode = 401, message = "User not found", data = (object)null });
            }

            var office = user.Offices;

            if (office == null || !office.IsActive)
            {
                return BadRequest(new { statusCode = 400, message = "User's office not found or inactive", data = (object)null });
            }

            // Calculate if the user is within the office radius
            double distance = CalculateDistance(office.Latitude, office.Longitude, request.Latitude, request.Longitude);
            bool isOnLocation = distance <= office.Radius;

            if (request.IsOnLocation && !isOnLocation)
            {
                return BadRequest(new { statusCode = 400, message = "You are outside the location radius.", data = (object)null });
            }

            // Handle new check-in
            if (request.IsCheckedIn == 0)
            {
                // Check if there is an active check-in (without a checkout)
                var activeCheckIn = await _context.Attendance
                    .Where(a => a.UserId == ptslId && a.Date == DateTime.Today && a.IsCheckedIn && !a.IsCheckedOut)
                    .OrderByDescending(a => a.CheckIn)
                    .FirstOrDefaultAsync();

                if (activeCheckIn != null)
                {
                    // Automatically checkout the previous check-in with current time
                    activeCheckIn.CheckOut = DateTime.Now;
                    activeCheckIn.IsCheckedOut = true;
                    await _context.SaveChangesAsync();
                }

                // Now proceed to create a new check-in
                var newAttendance = new Attendance
                {
                    UserId = ptslId,
                    Users = user,
                    Date = DateTime.Now.Date,
                    IsOnLocation = isOnLocation,
                    CheckIn = DateTime.Now,
                    IsCheckedIn = true,
                    IsCheckedOut = false,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    IsActive = true
                };

                _context.Attendance.Add(newAttendance);

                // If the user is outside the office radius, record OtherAttendance
                if (request.Title != null)
                {
                    var otherAttendance = new OtherAttendance
                    {
                        AttendanceId = newAttendance.Id,
                        Attendance = newAttendance,
                        Image = request.Image,
                        Title = request.Title ?? "Attended from other place",
                        Description = request.Description,
                        Latitude = request.Latitude,
                        Longitude = request.Longitude,
                        IsActive = true
                    };

                    _context.OtherAttendance.Add(otherAttendance);
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    statusCode = 200,
                    message = "Checked in successfully",
                    data = new
                    {
                        date = newAttendance.Date,
                        userId = newAttendance.UserId,
                        checkIn = newAttendance.CheckIn,
                        checkOut = (DateTime?)null,
                        latitude = newAttendance.Latitude,
                        longitude = newAttendance.Longitude,
                        //image = request.Image
                    }
                });
            }
            else if (request.IsCheckedIn == 1)
            {
                // Handle Check-Out
                var latestAttendance = await _context.Attendance
                    .Where(a => a.UserId == ptslId && a.Date == DateTime.Today && a.IsCheckedIn && !a.IsCheckedOut)
                    .OrderByDescending(a => a.CheckIn)
                    .FirstOrDefaultAsync();

                if (latestAttendance == null)
                {
                    return BadRequest(new { statusCode = 400, message = "No active check-in found for checkout.", data = (object)null });
                }

                if (latestAttendance.IsOnLocation == true && request.IsOnLocation == false)
                {
                    return BadRequest(new { StatusCode = 400, message = "You must checkout from office area since you checked in from office area" });
                    
                    //var errorLog = new ErrorLogs
                    //{
                    //    UserId = ptslId,
                    //    Date = DateTime.Now,
                    //    ErrorMessage = "User must checkout from office area since they checked in from office area",
                    //    Latitude = request.Latitude,
                    //    Longitude = request.Longitude,
                    //    IsActive = true
                    //};
                }

                latestAttendance.CheckOut = DateTime.Now;
                latestAttendance.IsCheckedOut = true;
                latestAttendance.IsCheckedIn = false;
                latestAttendance.CheckoutLatitude = request.Latitude;
                latestAttendance.CheckoutLongitude = request.Longitude;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    statusCode = 200,
                    message = "Checked out successfully",
                    data = new
                    {
                        date = latestAttendance.Date,
                        userId = latestAttendance.UserId,
                        checkIn = latestAttendance.CheckIn,
                        checkOut = latestAttendance.CheckOut,
                        latitude = latestAttendance.Latitude,
                        longitude = latestAttendance.Longitude,
                        checkOutLat = latestAttendance.CheckoutLatitude,
                        checkOutLong = latestAttendance.CheckoutLongitude
                    }
                });
            }
            else
            {
                return BadRequest(new { statusCode = 400, message = "Invalid IsCheckedIn value.", data = (object)null });
            }
        }

        // Helper method to calculate distance between two lat/long points
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371e3; // Earth's radius in meters
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

    public class AttendanceRequest
    {
        public byte[] Image { get; set; }= new byte[0];
        public string? Title { get; set; }
        public string? Description { get; set; }
        public required double Latitude { get; set; }
        public required double Longitude { get; set; }
        public required bool IsOnLocation { get; set; }
        public required int IsCheckedIn { get; set; }
    }
}
