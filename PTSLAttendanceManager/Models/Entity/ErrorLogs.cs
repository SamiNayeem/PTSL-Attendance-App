namespace PTSLAttendanceManager.Models.Entity
{
    public class ErrorLogs
    {
        public long Id { get; set; }

        // Identify the user using foreign key
        public long ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime ErrorOccuredAt { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
