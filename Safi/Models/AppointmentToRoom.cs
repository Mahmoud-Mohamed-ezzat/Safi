using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Safi.Models
{
    public class AppointmentToRoom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ?CreatedBy { get; set; } // UserId who created
        [ForeignKey("CreatedBy")]
        public virtual User ?CreatedByUser { get; set; }
        public string? DoctorId { get; set; } // DoctorId who primary Doctor
        [ForeignKey("DoctorId")]
        public virtual Doctor? Doctor { get; set; }
        public string? PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }
        public int ?BillId { get; set; }
        public int ?RoomId { get; set; }  
        [ForeignKey("RoomId")]
        public virtual Room? Room { get; set; }
        [ForeignKey("BillId")]
        public virtual Bill? Bill { get; set; }
        public DateTime? StartTime { get; set; }=DateTime.UtcNow;
        public DateTime? EndTime { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? TotalPrice { get; set; }
        public virtual ICollection<AppointmentPrice>? AppointmentPrices { get; set; } = new List<AppointmentPrice>();
    }
}
