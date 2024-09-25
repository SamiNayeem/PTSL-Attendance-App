namespace PTSLAttendanceManager.Models
{
    public class AttendanceHistoryDto
    {
        public string PtslId { get; set; }
        public string Name { get; set; }
        public string TeamName { get; set; }
        public DateTime Date { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public TimeSpan? CheckIn { get; set; }  
        public TimeSpan? CheckOut { get; set; }  
        public bool IsOnLocation { get; set; }
        public double? AttendanceLatitude { get; set; }
        public double? AttendanceLongitude { get; set; }
        public long? OtherAttendanceId { get; set; }
        public byte[]? Image { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        //public double? OtherAttendanceLatitude { get; set; }
        //public double? OtherAttendanceLongitude { get; set; }
    }


}
