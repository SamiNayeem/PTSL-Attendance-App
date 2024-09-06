using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PTSLAttendanceManager.Models.Entity
{
    public class Teams
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public required string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
