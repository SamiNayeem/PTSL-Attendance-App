using System.ComponentModel.DataAnnotations.Schema;

namespace PTSLAttendanceManager.Models.Entity
{
    public class ProxyLogs
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }

        // Foreign key for the ProxyGiver (PtslId as a foreign key)
        public required string ProxyGiverId { get; set; }

        // Foreign key for the AbsentPerson (PtslId as a foreign key)
        public required string AbsentPersonId { get; set; }

        // Navigation property for the ProxyGiver
        [ForeignKey(nameof(ProxyGiverId))]
        public required Users ProxyGiver { get; set; }

        // Navigation property for the AbsentPerson
        [ForeignKey(nameof(AbsentPersonId))]
        public required Users AbsentPerson { get; set; }

        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
