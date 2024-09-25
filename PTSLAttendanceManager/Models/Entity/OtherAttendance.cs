using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PTSLAttendanceManager.Models.Entity
{
    public class OtherAttendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        // Foreign key property for Attendance
        public required long AttendanceId { get; set; }

        // Navigation property with ForeignKey attribute
        [ForeignKey(nameof(AttendanceId))]
        public required Attendance Attendance { get; set; }

        public virtual byte[] Image { get; set; } =new byte[0];
        public required string Title { get; set; }
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public required double Latitude { get; set; }
        public required double Longitude { get; set; }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
