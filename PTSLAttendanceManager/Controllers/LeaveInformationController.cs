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
        [Authorize] // Ensure correct authorization
        public async Task<IActionResult> GetPendingLeave()
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

            // Retrieve the UserWiseLeave record for the given PtslId
            var userLeave = await _context.UserWiseLeave
                .Where(leave => leave.UserId == ptslId && leave.IsActive)
                .FirstOrDefaultAsync();

            if (userLeave == null)
            {
                return NotFound(new
                {
                    statusCode = 404,
                    message = "No pending leaves found for the given PtslId",
                    data = new object() { }
                });
            }

            // Retrieve all active leave types from the database
            var leaveTypes = await _context.LeaveType
                .Where(type => type.IsActive)
                .ToListAsync();

            // Create a list to hold the dynamic response
            var leaveData = new List<object>();

            foreach (var leaveType in leaveTypes)
            {
                long remaining = 0;

                // Match the leave type and get the corresponding remaining leave from UserWiseLeave
                switch (leaveType.Type.ToLower())
                {
                    case "earned":
                        remaining = userLeave.PendingEarnedLeave;
                        break;
                    case "casual":
                        remaining = userLeave.PendingCasualLeave;
                        break;
                    case "sick":
                        remaining = userLeave.PendingSickLeave;
                        break;
                    case "maternity":
                        remaining = userLeave.PendingMaternityLeave;
                        break;
                }

                // Add the leave type data to the list
                leaveData.Add(new
                {
                    id = leaveType.Id,
                    label = leaveType.Type,
                    total = leaveType.TotalLeaveDays,
                    remaining = remaining
                });
            }

            return Ok(new
            {
                statusCode = 200,
                message = "Pending leaves retrieved successfully",
                data = leaveData
            });
        }
    }
}
