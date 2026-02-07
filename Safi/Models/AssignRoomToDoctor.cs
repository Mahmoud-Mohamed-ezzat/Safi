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
        public string? DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor? Doctor { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        //Daily Shift
        public TimeOnly Start_Time { get; set; } = new TimeOnly();
        public TimeOnly End_Time { get; set; } = new TimeOnly();
    }
}
