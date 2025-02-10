namespace PTSLAttendanceManager.Models
{
    public class GetLeaveApplicationHistory
    {
        //public string PtslId { get; set; }
        //public string Name { get; set; }
        //public DateOnly FromDate { get; set; }
        //public DateOnly ToDate { get; set; }
        //public long TotalDays { get; set; }
        //public string AssignedTo { get; set; }
        //public string Status { get; set; }
        //public DateTime? ApprovedByPMAt { get; set; } // Ensure this is nullable
        //public DateTime? ApprovedByHRAt { get; set; } // Ensure this is nullable
        public long Id { get; set; }
        public string PtslId { get; set; }
        public string Designation { get; set; }
        public string Name { get; set; }
        public DateTime ApplyingDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public float TotalDays { get; set; }
        public string Reason { get; set; }
        public string LeaveType { get; set; }
        public string LeaveDuration { get; set; }
        public string? AssignedTo { get; set; }
        public string AddressDuringLeave { get; set; }
        public bool ProjectManagerApproval { get; set; }
        public string? ApprovedBy { get; set; }
        public string? ApprovalPMName { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public bool HRApproval { get; set; }
        public float PendingSickLeave { get; set; }
        public float PendingEarnedLeave { get; set; }
        public float PendingCasualLeave { get; set; }
        public float PendingLeave { get; set; }
        public string Status { get; set; }
    }


}

