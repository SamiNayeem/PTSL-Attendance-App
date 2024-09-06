namespace PTSLAttendanceManager.Models
{
    public class UserConfigDto
    {
        public string PtslId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public string Office { get; set; }
        public string OfficeAddress { get; set; }
        public string TeamName { get; set; }
        public string Role { get; set; }
    }

}
