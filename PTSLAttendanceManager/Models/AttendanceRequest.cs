namespace PTSLAttendanceManager.Models
{
    public class AttendanceRequest
    {
        public required string UserId { get; set; }
        public required double Latitude { get; set; }
        public required double Longitude { get; set; }
    }
}
