using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PTSLAttendanceManager.Models.Entity
{
    public class LeaveApplication
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public required string UserId { get; set; }
        public required Users User { get; set; }
        public long UserWiseLeaveId { get; set; }
        public required UserWiseLeave UserWiseLeave { get; set; }
        public required long LeaveDurationId { get; set; }
        public required LeaveDuration LeaveDuration { get; set; }
        public required DateOnly ApplyingDate { get; set; }

        public long LeaveTypeId { get; set; }
        public required LeaveType LeaveType { get; set; }
        public required DateOnly FromDate { get; set; }
        public required DateOnly ToDate { get; set; }
        public required long TotalDays { get; set; }
        public string? Reason { get; set; }
        public string? AssignedTo { get; set; }
        public Users? AssignedUser { get; set; }
        public String? AddressDuringLeave { get; set; }
        public bool IsApprovedByProjectManager { get; set; } = false;
        public DateTime? ApprovedByProjectManagerAt { get; set; }
        public bool IsApprovedByHR { get; set; } = false;
        public DateTime? ApprovedByHRAt { get; set; }
        public long? ApprovalStatusId { get; set; }
        public required ApprovalStatus ApprovalStatus { get; set; }
        public required string Status { get; set; } = "Pending";
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;
        
    }
}
