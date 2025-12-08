using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Safi.Models
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Number { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        // Navigation properties
        public virtual ICollection<AssignRoomToDoctor> ?AssignRoomToDoctors { get; set; } = new List<AssignRoomToDoctor>();
        public virtual ICollection<AppointmentToRoom> ?Appointments { get; set; } = new List<AppointmentToRoom>();
    }
}
