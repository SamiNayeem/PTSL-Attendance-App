namespace PTSLAttendanceManager.Models.Entity
{
    public class VersionControl
    {
        public long Id { get; set; }
        public required string Version { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public bool IsUpdateAvailable { get; set; }
        public bool IsForceUpdateAvailable { get; set; }
    }
}


