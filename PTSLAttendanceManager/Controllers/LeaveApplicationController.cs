using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PTSLAttendanceManager.Data;
using PTSLAttendanceManager.Models.Entity;
using System;
using System.Linq;
using System.Threading.Tasks;
using PTSLAttendanceManager.Models;
using Microsoft.Data.SqlClient;

namespace PTSLAttendanceManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveApplicationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaveApplicationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("ApplyLeave")]
        [Authorize] // Ensure correct authorization
        public async Task<IActionResult> ApplyLeave([FromBody] LeaveApplicationRequest request)
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

            // Check for conflicts with the assigned person’s leave on the specified dates
            var assignedPersonLeaveConflict = await _context.LeaveApplication
                .Where(leave => leave.AssignedTo == request.AssignedTo &&
                                leave.IsActive &&
                                (leave.FromDate <= request.ToDate && leave.ToDate >= request.FromDate))
                .AnyAsync();

            if (assignedPersonLeaveConflict)
            {
                return BadRequest(new
                {
                    statusCode = 400,
                    message = "Assigned person already has leave on the specified days.",
                    data = new object() { }
                });
            }

            // Validate the leave duration
            if (!IsLeaveApplicationValid(request, out string errorMessage))
            {
                return BadRequest(new
                {
                    statusCode = 400,
                    message = errorMessage,
                    data = new object() { }
                });
            }

            // Ensure the correct type is used for ApprovalStatusId
            long approvalStatusId = 3;

            // Retrieve the ApprovalStatus or create a default
            var approvalStatus = await _context.ApprovalStatus.FindAsync(approvalStatusId)
                                 ?? new ApprovalStatus { Id = approvalStatusId, Status = "On Hold", IsActive = true };

            // Create a new leave application record
            var newLeaveApplication = new LeaveApplication
            {
                UserId = ptslId,
                User = userLeave.User,
                UserWiseLeaveId = userLeave.Id,
                UserWiseLeave = userLeave,
                LeaveDurationId = request.LeaveDurationId,
                LeaveDuration = await _context.LeaveDuration.FindAsync(request.LeaveDurationId),
                ApplyingDate = DateOnly.FromDateTime(DateTime.UtcNow),
                LeaveTypeId = request.LeaveTypeId,
                LeaveType = await _context.LeaveType.FindAsync(request.LeaveTypeId),
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                TotalDays = request.TotalDays,
                Reason = request.Reason,
                AssignedTo = request.AssignedTo,
                AssignedUser = await _context.Users.FirstOrDefaultAsync(u => u.PtslId == request.AssignedTo),
                AddressDuringLeave = request.AddressDuringLeave,
                IsApprovedByProjectManager = false,
                IsApprovedByHR = false,
                ApprovalStatus = approvalStatus,
                ApprovalStatusId = approvalStatusId,
                Status = "Pending",
                Remarks = null,
                IsActive = true
            };

            // Save the leave application
            _context.LeaveApplication.Add(newLeaveApplication);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                statusCode = 200,
                message = "Leave applied successfully",
                data = newLeaveApplication
            });
        }


        //private bool IsLeaveApplicationValid(LeaveApplicationRequest request, out string errorMessage)
        //{
        //    errorMessage = string.Empty;

        //    // Basic validation for total days
        //    if (request.TotalDays <= 0)
        //    {
        //        errorMessage = "Total leave days must be greater than zero.";
        //        return false;
        //    }

        //    // Ensure from-date is earlier than or equal to to-date
        //    if (request.FromDate > request.ToDate)
        //    {
        //        errorMessage = "FromDate cannot be after ToDate.";
        //        return false;
        //    }

        //    return true;
        //}


        [HttpPost("PMApproval")]
        [Authorize]
        public async Task<IActionResult> PMApprovalLeave([FromBody] PMApprovalRequest request)
        {
            var ptslId = User.FindFirst("PtslId")?.Value;
            var user = await _context.Users
                .Where(u => u.PtslId == ptslId)
                .Select(u => new { u.RoleId })
                .FirstOrDefaultAsync();

            if (user == null || user.RoleId != 4) // Ensure only RoleId = 4 can approve
            {
                return Unauthorized(new { statusCode = 401, message = "Unauthorized access", data = new object() { } });
            }

            // Retrieve the leave application
            var leaveApplication = await _context.LeaveApplication
                .Include(la => la.UserWiseLeave)
                .FirstOrDefaultAsync(la => la.Id == request.LeaveApplicationId && la.IsActive);

            if (leaveApplication == null)
            {
                return NotFound(new
                {
                    statusCode = 404,
                    message = "Leave application not found",
                    data = new object() { }
                });
            }

            if (request.Flag == 1) // Approve
            {
                bool leaveDeducted = DeductLeaveDays(leaveApplication.UserWiseLeave, leaveApplication.LeaveTypeId, leaveApplication.TotalDays);

                if (!leaveDeducted)
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "Insufficient leave days for the specified leave type.",
                        data = new object() { }
                    });
                }

                leaveApplication.IsApprovedByProjectManager = true;
                leaveApplication.ApprovedByProjectManagerAt = DateTime.UtcNow;
                leaveApplication.Status = "Approved";
            }
            else if (request.Flag == 0) // Reject
            {
                leaveApplication.Status = "Rejected";
                leaveApplication.IsActive = false;
            }
            else
            {
                return BadRequest(new
                {
                    statusCode = 400,
                    message = "Invalid flag value. Use 1 for approval and 0 for rejection.",
                    data = new object() { }
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                statusCode = 200,
                message = request.Flag == 1 ? "Approved" : "Rejected",
                data = new
                {
                    leaveApplication.Id,
                    leaveApplication.Status,
                    leaveApplication.IsApprovedByProjectManager,
                    leaveApplication.ApprovedByProjectManagerAt
                }
            });
        }

        private bool DeductLeaveDays(UserWiseLeave userLeave, long leaveTypeId, long totalDays)
        {
            switch (leaveTypeId)
            {
                case 1: // Earned Leave
                    if (userLeave.PendingEarnedLeave >= totalDays)
                    {
                        userLeave.PendingEarnedLeave -= totalDays;
                        return true;
                    }
                    break;
                case 2: // Casual Leave
                    if (userLeave.PendingCasualLeave >= totalDays)
                    {
                        userLeave.PendingCasualLeave -= totalDays;
                        return true;
                    }
                    break;
                case 3: // Sick Leave
                    if (userLeave.PendingSickLeave >= totalDays)
                    {
                        userLeave.PendingSickLeave -= totalDays;
                        return true;
                    }
                    break;
                case 4: // Maternity Leave
                    if (userLeave.PendingMaternityLeave >= totalDays)
                    {
                        userLeave.PendingMaternityLeave -= totalDays;
                        return true;
                    }
                    break;
                default:
                    return false; // Return false if LeaveTypeId doesn't match any case
            }
            return false;
        }
        private bool IsLeaveApplicationValid(LeaveApplicationRequest request, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Basic validation for total days
            if (request.TotalDays <= 0)
            {
                errorMessage = "Total leave days must be greater than zero.";
                return false;
            }

            // Ensure from-date is earlier than or equal to to-date
            if (request.FromDate > request.ToDate)
            {
                errorMessage = "FromDate cannot be after ToDate.";
                return false;
            }

            return true;
        }

        // Need to complete HR Acknowledgement and history
        [HttpPost("HRApproval")]
        [Authorize]
        public async Task<IActionResult> HRApprovalLeave([FromBody] PMApprovalRequest request)
        {
            var ptslId = User.FindFirst("PtslId")?.Value;
            var user = await _context.Users
                .Where(u => u.PtslId == ptslId)
                .Select(u => new { u.RoleId })
                .FirstOrDefaultAsync();

            if (user == null || user.RoleId != 6) // Ensure only RoleId = 6 can approve
            {
                return Unauthorized(new { statusCode = 401, message = "Unauthorized access", data = new object() { } });
            }

            // Retrieve the leave application
            var leaveApplication = await _context.LeaveApplication
                .Include(la => la.UserWiseLeave)
                .FirstOrDefaultAsync(la => la.Id == request.LeaveApplicationId && la.IsActive);

            if (leaveApplication == null)
            {
                return NotFound(new
                {
                    statusCode = 404,
                    message = "Leave application not found",
                    data = new object() { }
                });
            }

            if (request.Flag == 1) // Approve
            {


                leaveApplication.IsApprovedByHR = true;
                leaveApplication.ApprovedByHRAt = DateTime.UtcNow;
                leaveApplication.Status = "Approved, Acknowledged";
            }
            // HR can not reject leave approval
            //else if (request.Flag == 0) // Reject
            //{
            //    leaveApplication.Status = "Rejected by PM";
            //    leaveApplication.IsActive = false;
            //}
            else
            {
                return BadRequest(new
                {
                    statusCode = 400,
                    message = "Invalid flag value. Use 1 for acknowledgement.",
                    data = new object() { }
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                statusCode = 200,
                message = "Approved, Acknowledged",
                data = new
                {
                    leaveApplication.Id,
                    leaveApplication.Status,
                    leaveApplication.IsApprovedByHR,
                    leaveApplication.ApprovedByHRAt
                }
            });
        }

        [HttpGet("GetLeaveApplicationsHistory")]
        [Authorize] // Ensure correct authorization
        public async Task<IActionResult> GetLeaveApplicationHistory()
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
            //var user = await _context.Users
            //    .Where(u => u.PtslId == userId)
            //    .Select(u => new { u.RoleId })
            //    .FirstOrDefaultAsync();

            //if (user == null)
            //{
            //    return Unauthorized(new
            //    {
            //        statusCode = 401,
            //        message = "Invalid token or role",
            //        data = (object)null
            //    });
            //}

            //long roleId = user.RoleId;
            var userIdParam = new SqlParameter("@UserId", userId);
            

            try
            {
                // Fetch data using the stored procedure
                var result = await _context.Database.SqlQueryRaw<GetLeaveApplicationHistory>(
    "EXEC GetLeaveApplicationHistory @UserId", userIdParam
).ToListAsync();

                if (result == null || result.Count == 0)
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No leave applications found",
                        data = new List<GetLeaveApplicationHistory>()
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
        [HttpPost("CancelLeave")]
        [Authorize]
        public async Task<IActionResult> CancelLeave([FromBody] CancelLeaveRequest request)
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

            // Retrieve the leave application for the given LeaveApplicationId and UserId
            var leaveApplication = await _context.LeaveApplication
                .Where(la => la.Id == request.LeaveApplicationId && la.UserId == ptslId && la.IsActive)
                .FirstOrDefaultAsync();

            if (leaveApplication == null)
            {
                return NotFound(new
                {
                    statusCode = 404,
                    message = "Leave application not found or cannot be canceled",
                    data = new object() { }
                });
            }

            // Check if the cancellation is allowed (current date must be before FromDate)
            if (DateOnly.FromDateTime(DateTime.UtcNow) >= leaveApplication.FromDate)
            {
                return BadRequest(new
                {
                    statusCode = 400,
                    message = "Leave application cannot be canceled on or after the start date.",
                    data = new object() { }
                });
            }

            // Mark the leave application as inactive
            leaveApplication.IsActive = false;
            leaveApplication.Status = "Canceled";

            // Save changes
            await _context.SaveChangesAsync();

            return Ok(new
            {
                statusCode = 200,
                message = "Leave application canceled successfully",
                data = new
                {
                    leaveApplication.Id,
                    leaveApplication.IsActive
                }
            });
        }


    }
}

  