namespace PTSLAttendanceManager.Models
{
    public class AttendanceHistory
    {
        public string PtslId { get; set; }
        public long TeamID { get; set; }
        public long RoleId { get; set; }
        public DateOnly Date { get; set; }
        public long Month { get; set; }
        public long Year { get; set; }
    }
}
