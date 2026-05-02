using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Safi.Models
{
    public class Attendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public string status { get; set; } //"attend,Late,Off,Abscent"
        public string? Notes { get; set; }
        public string UserrId { get; set; }
        public int ShiftId { get; set; } // from assign rooms to doctor
        [ForeignKey("UserrId")]
        public virtual User Doctor { get; set; }
        [ForeignKey("ShiftId")]
        public virtual Shift DoctorShift { get; set; }
       
    }
}
