using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Safi.Models
{
    public struct RoomStatus
    {
        public const string Available = "Available";
        public const string Busy = "Busy";
    }
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Number { get; set; }
        public string Status { get; set; } = RoomStatus.Available;
        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        // Navigation properties
        public virtual ICollection<AssignRoomToDoctor> ?AssignRoomToDoctors { get; set; } = new List<AssignRoomToDoctor>();
        public virtual ICollection<AppointmentToRoom> ?Appointments { get; set; } = new List<AppointmentToRoom>();
    }
}

