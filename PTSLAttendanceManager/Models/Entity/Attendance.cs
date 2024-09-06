using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSLAttendanceManager.Models.Entity
{
    public class Attendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public required DateTime Date { get; set; }

        // Foreign key property for Users
        public required string UserId { get; set; }  

        // Navigation property with ForeignKey attribute
        [ForeignKey(nameof(UserId))]
        public required Users Users { get; set; }

        public required DateTime CheckIn { get; set; }
        public required DateTime CheckOut { get; set; }

        public bool IsActive { get; set; } = true;

        public required double Latitude { get; set; }
        public required double Longitude { get; set; }  // Fixed spelling from `Logitude` to `Longitude`

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
