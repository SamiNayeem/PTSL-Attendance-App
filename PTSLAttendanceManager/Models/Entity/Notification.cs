using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PTSLAttendanceManager.Models.Entity
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public required DateTime Date { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public bool IsActive { get; set; } = true;

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
