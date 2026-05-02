using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Safi.Models
{
    public class AppointmentPrice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int AppointmentId { get; set; }
        [ForeignKey("AppointmentId")]
        public  virtual AppointmentToRoom? Appointment { get; set; }
        public int PriceId { get; set; }
        [ForeignKey("PriceId")]
        public virtual Prices prices { get; set; }
        public DateTime Start_Time { get; set; }
        public DateTime End_Time { get; set; }
        public float TotalHours { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }
    }
}
