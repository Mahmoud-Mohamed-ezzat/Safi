using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Safi.Models
{
    public class AssignWorks
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public int? RoomId { get; set; } // Nullable if it will assign directly to Staff 
        [ForeignKey("RoomId")]
        public virtual Room? Room { get; set; }
        public string? userId { get; set; }
        [ForeignKey("userId")]
        public virtual User? user { get; set; } // can assign to Doctor or Staff or Nurse
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int ShiftId { get; set; }
        [ForeignKey("ShiftId")]
        public virtual Shift? Shift { get; set; }
    }
}
