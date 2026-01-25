using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Safi.Models
{
    public class AssignRoomToDoctor
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public int? RoomId { get; set; }
        [ForeignKey("RoomId")]
        public virtual Room? Room { get; set; }

        public int? AppointmentToRoomId { get; set; }
        [ForeignKey("AppointmentToRoomId")]
        public virtual AppointmentToRoom? AppointmentToRoom { get; set; }

        public string? DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor? Doctor { get; set; }

        public DateTime Time { get; set; } = DateTime.Now;
    }
}
