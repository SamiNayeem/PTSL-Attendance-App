using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PTSLAttendanceManager.Data;
using PTSLAttendanceManager.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace PTSLAttendanceManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileConfigController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfileConfigController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetUserProfile")]
        [Authorize] // Ensure correct authorization
        public async Task<IActionResult> GetProfileConfig()
        {
            // Retrieve the PtslId from the JWT token claims
            var ptslId = User.FindFirst("PtslId")?.Value;

            if (string.IsNullOrEmpty(ptslId))
            {
                return Unauthorized(new
                {
                    statusCode = 401,
                    message = "Invalid token",
                    data = new object() { }
                });
            }

            var ptslIdParam = new SqlParameter("@PtslId", ptslId);

            try
            {
                // Fetch all data using the stored procedure and handle it in-memory
                var result = await _context.UserConfigDtos
                    .FromSqlRaw("EXEC Config @PtslId", ptslIdParam)
                    .AsNoTracking() // Avoid Entity Framework tracking issues
                    .ToListAsync(); // Fetch all results into a list

                var userProfile = result.FirstOrDefault(); // Get the first result or null

                if (userProfile == null)
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "User not found",
                        data = new object() { }
                    });
                }

                // Returning the single object in the expected format
                return Ok(new
                {
                    statusCode = 200,
                    message = "Data retrieved successfully",
                    data = new
                    {
                        ptslId = userProfile.PtslId,
                        name = userProfile.Name,
                        phone = userProfile.Phone,
                        designation = userProfile.Designation,
                        email = userProfile.Email,
                        isActive = userProfile.IsActive,
                        office = userProfile.Office,
                        officeAddress = userProfile.OfficeAddress,
                        officeLatitude = userProfile.OfficeLatitude,
                        officeLongitude = userProfile.OfficeLongitude,
                        officeRadius = userProfile.OfficeRadius,
                        teamName = userProfile.TeamName,
                        role = userProfile.Role
                    }
                });
            }
            catch (Exception ex)
            {
                // Catch any exceptions and return an appropriate error response
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving data.",
                    error = ex.Message
                });
            }
        }
    }
}
