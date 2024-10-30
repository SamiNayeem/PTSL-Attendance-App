namespace PTSLAttendanceManager.Models
{

    public class LeaveApplicationRequest
    {
        public long LeaveTypeId { get; set; }
        public long LeaveDurationId { get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public long TotalDays { get; set; }
        public string? Reason { get; set; }
        public string? AssignedTo { get; set; }
        public string? AddressDuringLeave { get; set; }
    }

}
