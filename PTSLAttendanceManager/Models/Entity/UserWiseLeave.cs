namespace PTSLAttendanceManager.Models.Entity
{
    public class UserWiseLeave
    {
        public long Id { get; set; }
        public required string UserId { get; set; }
        public required Users User { get; set; }
        public long PendingEarnedLeave { get; set; }
        public long PendingCasualLeave { get; set; }
        public long PendingSickLeave { get; set; }
        public long PendingMaternityLeave { get; set; }
        public bool IsActive { get; set; } = true;


    }
}
