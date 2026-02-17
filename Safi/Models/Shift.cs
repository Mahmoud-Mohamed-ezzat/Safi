using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Safi.Models
{
    public class Shift
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        //navigation property (1=>many)
        public virtual ICollection<AssignRoomToDoctor>? AssignRoomToDoctors { get; set; } = new List<AssignRoomToDoctor>();


    }
}
