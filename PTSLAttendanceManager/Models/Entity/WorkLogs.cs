using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PTSLAttendanceManager.Models.Entity
{
    public class WorkLogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public DateOnly Date { get; set; }
        public required string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public required Users Users { get; set; }
        public required DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; } = null;
        public TimeSpan TotalSpendingHour { get; set; }
        public TimeSpan TotalWorkingHour { get; set; }
    }
}


// This is my Attendance Class. From this class I want to create another class named WorkLogs. It will have the following properties: Id, Date, UserId, CheckIn, CheckOut, TotalSpendingHour which will be calculated from checkin and checkout