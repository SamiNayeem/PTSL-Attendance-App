using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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
        public DbSet<ProxyLogs> ProxyLogs { get; set; }
        public DbSet<ScheduleTime> ScheduleTime { get; set; }
        public DbSet<LeaveType> LeaveType { get; set; }
        public DbSet<UserWiseLeave> UserWiseLeave { get; set; }
        public DbSet<LeaveApplication> LeaveApplication { get; set; }
        public DbSet<LeaveDuration> LeaveDuration { get; set; }
        public DbSet<ApprovalStatus> ApprovalStatus { get; set; }

        public DbSet<VersionControl> VersionControl { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProxyLogs>()
                .HasOne(pl => pl.ProxyGiver)
                .WithMany() // No reverse navigation needed
                .HasForeignKey(pl => pl.ProxyGiverId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading delete

            modelBuilder.Entity<ProxyLogs>()
                .HasOne(pl => pl.AbsentPerson)
                .WithMany() // No reverse navigation needed
                .HasForeignKey(pl => pl.AbsentPersonId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading delete

            // LeaveType Configuration
            modelBuilder.Entity<LeaveType>()
                .HasMany<LeaveApplication>()
                .WithOne(la => la.LeaveType)
                .OnDelete(DeleteBehavior.Restrict);


            // LeaveApplication Configuration
            modelBuilder.Entity<LeaveApplication>()
                .HasOne(la => la.User)
                .WithMany()
                .HasForeignKey(la => la.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeaveApplication>()
                .HasOne(la => la.AssignedUser)
                .WithMany()
                .HasForeignKey(la => la.AssignedTo)
                .OnDelete(DeleteBehavior.Restrict);



            base.OnModelCreating(modelBuilder);
        }

    }
}
