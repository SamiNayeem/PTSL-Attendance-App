namespace PTSLAttendanceManager.Models.Entity
{
    public class UserWiseLeave
    {
        public long Id { get; set; }
        public required string UserId { get; set; }
        public required Users User { get; set; }
        public float PendingEarnedLeave { get; set; }
        public float PendingCasualLeave { get; set; }
        public float PendingSickLeave { get; set; }
        public float PendingMaternityLeave { get; set; }
        public bool IsActive { get; set; } = true;


    }
}
