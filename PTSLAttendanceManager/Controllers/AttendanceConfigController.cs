﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTSLAttendanceManager.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PTSLAttendanceManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceConfigController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AttendanceConfigController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Config")]
        [Authorize]
        public async Task<IActionResult> GetProfileConfig()
        {
            try
            {
                var ptslId = User.FindFirst("PtslId")?.Value;

                if (string.IsNullOrEmpty(ptslId))
                {
                    return Unauthorized(new { statusCode = 401, message = "Invalid token", data = (object)null });
                }

                // Fetch all records for the current day
                var attendanceConfig = await _context.Database.SqlQueryRaw<AttendanceConfigResult>("EXEC AttendanceConfig @PtslId = {0}", ptslId)
                                     .ToListAsync();

                // Get the first entry for office data regardless of checkin/checkout
                var result = attendanceConfig.FirstOrDefault();

                // Check if attendanceConfig contains any records
                if (result == null)
                {
                    return NotFound(new { statusCode = 404, message = "No attendance data found for today" });
                }

                // Gather all CheckIn and CheckOut times
                var checkInCheckOutTimes = attendanceConfig
                    .Where(a => a.CheckInTime.HasValue || a.CheckOutTime.HasValue)
                    .Select(a => new
                    {
                        CheckIn = a.CheckInTime,
                        CheckOut = a.CheckOutTime
                    })
                    .ToList();

                return Ok(new
                {
                    statusCode = 200,
                    message = "Attendance config retrieved successfully",
                    data = new
                    {
                        OfficeLat = result.OfficeLatitude,
                        OfficeLong = result.OfficeLongitude,
                        OfficeRadius = result.OfficeRadius,
                        Date = result.AttendanceDate,
                        CheckInCheckOutList = checkInCheckOutTimes  // List of CheckIn and CheckOut times
                    }
                });
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving attendance data.",
                    error = ex.Message
                });
            }
        }

        // Define the AttendanceConfigResult model that matches your stored procedure result
        public class AttendanceConfigResult
        {
            public double OfficeLatitude { get; set; }
            public double OfficeLongitude { get; set; }
            public long OfficeRadius { get; set; }
            public DateTime? AttendanceDate { get; set; }
            public DateTime? CheckInTime { get; set; }
            public DateTime? CheckOutTime { get; set; }
        }
    }
}
