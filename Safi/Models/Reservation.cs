using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Safi.Models
{
    public class Reservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient ? Patient { get; set; } 
        public string DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; } = null!;
        public DateTime Time { get; set; }
        public string? Status { get; set; }  // "reserved", "completed", "un-reserved"
        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
    }
}
