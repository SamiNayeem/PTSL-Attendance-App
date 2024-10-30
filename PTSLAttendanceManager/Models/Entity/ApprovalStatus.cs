namespace PTSLAttendanceManager.Models.Entity
{
    public class ApprovalStatus
    {
        public long Id { get; set; }
        public string? Status { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
