namespace PTSLAttendanceManager.Models
{
    public class AttendanceConfigResult
    {
        public double OfficeLatitude { get; set; }
        public double OfficeLongitude { get; set; }
        public long OfficeRadius { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
    }
}
