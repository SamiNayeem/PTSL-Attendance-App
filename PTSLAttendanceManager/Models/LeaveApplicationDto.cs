namespace PTSLAttendanceManager.Models
{
    public class LeaveApplicationDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime ApplyingDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long TotalDays { get; set; }
        public string Reason { get; set; }
        public string LeaveType { get; set; }
        public string LeaveDuration { get; set; }
        public string AssignedTo { get; set; }
        public string AddressDuringLeave { get; set; }
        public bool ProjectManagerApproval { get; set; }
        public bool HRApproval { get; set; }
        public long PendingSickLeave { get; set; }
        public long PendingEarnedLeave { get; set; }
        public long PendingCasualLeave { get; set; }
    }

}
