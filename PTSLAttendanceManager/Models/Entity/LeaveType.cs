using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTSLAttendanceManager.Models.Entity
{
    public class LeaveType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public required string Type { get; set; }
        public required float TotalLeaveDays { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
