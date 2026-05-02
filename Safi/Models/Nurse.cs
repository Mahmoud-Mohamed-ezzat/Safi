using System.ComponentModel.DataAnnotations.Schema;

namespace Safi.Models
{
    public class Nurse: User
    {
        public string University { get; set; } // University attended

        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }
        public virtual ICollection<AssignWorks>? AssignNursesToRooms { get; set; } = new List<AssignWorks>();
    }
}
