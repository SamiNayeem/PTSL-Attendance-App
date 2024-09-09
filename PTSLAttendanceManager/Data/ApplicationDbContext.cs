using Microsoft.EntityFrameworkCore;
using PTSLAttendanceManager.Models;
using PTSLAttendanceManager.Models.Entity;

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


        public DbSet<UserConfigDto> UserConfigDtos { get; set; }

        public DbSet<AttendanceHistory> AttendanceHistory { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Optional: You can configure this DbSet to be ignored for migrations, as it's not a table
            builder.Entity<UserConfigDto>().HasNoKey();  // Indicates this entity has no primary key
            builder.Entity<AttendanceHistory>().HasNoKey();
        }

    }
}
