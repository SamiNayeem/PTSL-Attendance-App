using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSLAttendanceManager.Models.Entity
{
    public class Users
    {
        // Primary key property
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]  // This ensures that the primary key is not auto-generated
        public required string PtslId { get; set; }

        public required string Name { get; set; }
        public required string Phone { get; set; }
        public required string Designation { get; set; }

        // Foreign key property
        public required long OfficeId { get; set; }

        // Navigation property with ForeignKey attribute
        [ForeignKey(nameof(OfficeId))]
        public required Offices Offices { get; set; }

        public DateOnly? JoiningDate { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? DeviceUId { get; set; }
        public string? DeviceModel { get; set; }
        public DateOnly? LeavingDate { get; set; }
        public long RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public Roles? Roles { get; set; }

        public long? TeamId { get; set; }

        [ForeignKey(nameof(TeamId))]
        public Teams? Teams { get; set; }


        public long UserType { get; set; }


        public bool IsActive { get; set; } = true;

        public string? SessionInfo { get; set; }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
