using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Safi.Models
{
    public class Bill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime st_Date { get; set; }= DateTime.UtcNow;
        public DateTime ? end_Date { get; set; }
        public string Status { get; set; }   // "open" , "closed"
        public string ?Details { get; set; }
        public string currency { get; set; } = "EGP";
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; } = 0;
        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        public virtual ICollection<AppointmentToRoom> ? Appointments { get; set; } = new List<AppointmentToRoom>();
        public virtual ICollection<Reservation> ? Reservations { get; set; } = new List<Reservation>();

    }
}
