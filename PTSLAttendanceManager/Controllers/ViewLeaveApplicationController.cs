using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using PTSLAttendanceManager.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTSLAttendanceManager.Models;

namespace PTSLAttendanceManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ViewLeaveApplicationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ViewLeaveApplicationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetLeaveApplications")]
        [Authorize] // Ensure correct authorization
        public async Task<IActionResult> GetLeaveApplications()
        {
            // Retrieve the UserId from JWT claims
            var userId = User.FindFirst("PtslId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    statusCode = 401,
                    message = "Invalid token or role",
                    data = (object)null
                });
            }

            // Get RoleId from the database
            var user = await _context.Users
                .Where(u => u.PtslId == userId)
                .Select(u => new { u.RoleId })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Unauthorized(new
                {
                    statusCode = 401,
                    message = "Invalid token or role",
                    data = (object)null
                });
            }

            long roleId = user.RoleId;
            var userIdParam = new SqlParameter("@UserId", userId);
            var roleIdParam = new SqlParameter("@RoleId", roleId);

            try
            {
                // Fetch data using the stored procedure
                var result = await _context.Database.SqlQueryRaw<LeaveApplicationDto>(
                    "EXEC GetLeaveApplications @UserId, @RoleId", userIdParam, roleIdParam
                ).ToListAsync();

                if (result == null || result.Count == 0)
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No leave applications found",
                        data = new List<LeaveApplicationDto>()
                    });
                }

                // Return list of leave applications
                return Ok(new
                {
                    statusCode = 200,
                    message = "Leave applications retrieved successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving leave applications.",
                    error = ex.Message
                });
            }
        }
    }


    
}
