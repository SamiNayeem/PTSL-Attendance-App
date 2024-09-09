//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using PTSLAttendanceManager.Data;
//using PTSLAttendanceManager.Models.Entity;
//using System.Security.Claims;
//using System.Threading.Tasks;

//namespace PTSLAttendanceManager.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class OtherAttendanceController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;

//        public OtherAttendanceController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpPost("OtherAttendance")]
//        public async Task<IActionResult> Create([FromBody] OtherAttendanceRequest request)
//        {
//            // Retrieve the user's PtslId from the JWT token
//            var ptslId = User.FindFirst("PtslId")?.Value;

//            if (ptslId == null)
//            {
//                return Unauthorized(new { statusCode = 401, message = "Invalid token" });
//            }

//            // Get the user entity
//            var user = await _context.Users.FirstOrDefaultAsync(u => u.PtslId == ptslId);

//            if (user == null)
//            {
//                return Unauthorized(new { statusCode = 401, message = "User not found" });
//            }

//            // Create a new attendance record
//            var attendance = new Attendance
//            {
//                UserId = ptslId,
//                Users = user,  // Set the Users navigation property
//                Date = DateTime.Now.Date,
//                CheckIn = DateTime.Now,  // Assuming CheckIn time is now
//                IsCheckedIn = true,
//                IsCheckedOut = false,
//                Latitude = request.Latitude,
//                Longitude = request.Longitude,
//                IsActive = true
//            };

//            _context.Attendance.Add(attendance);
//            await _context.SaveChangesAsync();

            
//            var otherAttendance = new OtherAttendance
//            {
//                AttendanceId = attendance.Id,  
//                Attendance = attendance,  
//                Image = request.Image,
//                Title = request.Title,
//                Description = request.Description,
//                Latitude = request.Latitude,
//                Longitude = request.Longitude 
//            };

//            _context.OtherAttendance.Add(otherAttendance);
//            await _context.SaveChangesAsync();

//            return Ok(new
//            {
//                statusCode = 200,
//                message = "Other attendance recorded successfully",
//                data = new
//                {
//                    otherAttendance.Id,
//                    otherAttendance.Image,
//                    otherAttendance.Title,
//                    otherAttendance.Description,
//                    otherAttendance.Latitude,
//                    otherAttendance.Longitude
//                }
//            });
//        }

//        [HttpPost("CheckOut")]
//        public async Task<IActionResult> CheckOut([FromBody] CheckOutRequest request)
//        {
//            // Retrieve the user's PtslId from the JWT token
//            var ptslId = User.FindFirst("PtslId")?.Value;

//            if (ptslId == null)
//            {
//                return Unauthorized(new { statusCode = 401, message = "Invalid token" });
//            }

//            // Find the most recent active attendance record for the user
//            var attendance = await _context.Attendance
//                .FirstOrDefaultAsync(a => a.UserId == ptslId && a.IsActive);

//            if (attendance == null)
//            {
//                return BadRequest(new { statusCode = 400, message = "No active attendance record found" });
//            }

//            // Find the corresponding other attendance record for the attendance
//            var otherAttendance = await _context.OtherAttendance
//                .FirstOrDefaultAsync(oa => oa.AttendanceId == attendance.Id);

//            if (otherAttendance == null)
//            {
//                return BadRequest(new { statusCode = 400, message = "No corresponding other attendance record found" });
//            }

//            // Update the checkout time in the attendance record
//            attendance.CheckOut = DateTime.Now;
//            attendance.IsActive = false; 

//            _context.Attendance.Update(attendance);

           
//            otherAttendance.IsActive = false; // Mark as inactive if needed

//            _context.OtherAttendance.Update(otherAttendance);

//            await _context.SaveChangesAsync();

//            return Ok(new
//            {
//                statusCode = 200,
//                message = "Checked out successfully",
//                data = new
//                {
//                    attendance.Id,
//                    attendance.Date,
//                    attendance.CheckIn,
//                    attendance.CheckOut,
//                    otherAttendance.Latitude,
//                    otherAttendance.Longitude
//                }
//            });
//        }
//    }

//    public class OtherAttendanceRequest
//    {
//        public string Image { get; set; }
//        public string Title { get; set; }
//        public string? Description { get; set; }
//        public double Latitude { get; set; }
//        public double Longitude { get; set; }
//    }

//    public class CheckOutRequest
//    {
//        public double Latitude { get; set; }
//        public double Longitude { get; set; }
//    }
//}
