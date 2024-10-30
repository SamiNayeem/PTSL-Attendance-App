﻿using Quartz;
using PTSLAttendanceManager.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace PTSLAttendanceManager.Jobs
{
    public class CheckOutUpdateJob : IJob
    {
        private readonly ApplicationDbContext _context;

        public CheckOutUpdateJob(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var today = DateTime.Today;

            // Check if today is Sunday to Thursday
            if (today.DayOfWeek >= DayOfWeek.Sunday && today.DayOfWeek <= DayOfWeek.Thursday)
            {
                // Get records where checkout is null
                var recordsToUpdate = await _context.Attendance
                    .Where(a => a.IsCheckedIn && !a.IsCheckedOut)
                    .ToListAsync();

                foreach (var record in recordsToUpdate)
                {
                    // Set checkout time to check-in time
                    record.CheckOut = record.CheckIn;
                    record.IsCheckedOut = true;
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
