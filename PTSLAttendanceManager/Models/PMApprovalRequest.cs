namespace PTSLAttendanceManager.Models
{
    public class PMApprovalRequest
    {
        public long LeaveApplicationId { get; set; }
        public int Flag { get; set; } // 1 for accept, 0 for reject
    }
}
