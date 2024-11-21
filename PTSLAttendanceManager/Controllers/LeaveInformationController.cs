using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PTSLAttendanceManager.Data;
using PTSLAttendanceManager.Models.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTSLAttendanceManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveInformationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaveInformationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("GetPendingLeave")]
        [Authorize]
        public async Task<IActionResult> GetPendingLeave([FromBody] GetPendingLeaveRequest request)
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

            // Determine which UserId to use for the query
            var userIdToQuery = string.IsNullOrEmpty(request.UserIdParam) ? ptslId : request.UserIdParam;

            // Retrieve the UserWiseLeave record for the given UserId
            var userLeave = await _context.UserWiseLeave
                .Where(leave => leave.UserId == userIdToQuery && leave.IsActive)
                .FirstOrDefaultAsync();

            if (userLeave == null)
            {
                return NotFound(new
                {
                    statusCode = 404,
                    message = $"No pending leaves found for the given UserId: {userIdToQuery}",
                    data = new object() { }
                });
            }

            // Retrieve all active leave types from the database
            var leaveTypes = await _context.LeaveType
                .Where(type => type.IsActive)
                .ToListAsync();

            // Create a list to hold the dynamic response
            var leaveData = leaveTypes.Select(leaveType =>
            {
                long remaining = leaveType.Type.ToLower() switch
                {
                    "earned" => userLeave.PendingEarnedLeave,
                    "casual" => userLeave.PendingCasualLeave,
                    "sick" => userLeave.PendingSickLeave,
                    "maternity" => userLeave.PendingMaternityLeave,
                    _ => 0
                };

                return new
                {
                    id = leaveType.Id,
                    label = leaveType.Type,
                    total = leaveType.TotalLeaveDays,
                    remaining
                };
            }).ToList();

            return Ok(new
            {
                statusCode = 200,
                message = "Pending leaves retrieved successfully",
                data = leaveData
            });
        }

    }
    public class GetPendingLeaveRequest
    {
        public string? UserIdParam { get; set; } // Optional UserId
    }

}
