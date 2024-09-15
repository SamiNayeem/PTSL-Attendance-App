using Microsoft.EntityFrameworkCore;
using PTSLAttendanceManager.Controllers;
using PTSLAttendanceManager.Models;
using PTSLAttendanceManager.Models.Entity;
using YourNamespace.Models;

namespace PTSLAttendanceManager.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Your custom DbSets
        public DbSet<Offices> Offices { get; set; }
        public DbSet<Teams> Teams { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Users> Users { get; set; }

        public DbSet<Attendance> Attendance { get; set; }
        public DbSet<OtherAttendance> OtherAttendance { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Otp> Otp { get; set; }


        public DbSet<UserConfigDto> UserConfigDtos { get; set; }

        public DbSet<AttendanceHistory> AttendanceHistory { get; set; }

        public DbSet<AttendanceConfigResult> AttendanceConfigResult { get; set; }
        public DbSet<AttendanceHistoryRequest> AttendanceHistoryRequest {get; set; }
        public DbSet<AttendanceHistoryDto> AttendanceHistoryDto { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Optional: You can configure this DbSet to be ignored for migrations, as it's not a table
            builder.Entity<UserConfigDto>().HasNoKey();  
            builder.Entity<AttendanceHistory>().HasNoKey();
            builder.Entity<AttendanceConfigResult>().HasNoKey();
            builder.Entity<AttendanceHistoryRequest>().HasNoKey();
            builder.Entity<AttendanceHistoryDto>().HasNoKey();
        }

    }
}
