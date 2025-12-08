using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Safi.Models
{
    public class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation properties
        public virtual ICollection<Doctor>? Doctors { get; set; } = new List<Doctor>();
        public virtual ICollection<Staff>? Staffs { get; set; } = new List<Staff>();
        public virtual ICollection<Patient>? Patients { get; set; } = new List<Patient>();
        public virtual ICollection<Room>? Rooms { get; set; } = new List<Room>();
        public virtual ICollection<ICU>? Icus { get; set; } = new List<ICU>();
        public virtual ICollection<Emergency>? Emergencies { get; set; } = new List<Emergency>();
        //public virtual ICollection<Ambulance> Ambulances { get; set; } = new List<Ambulance>();
    }
}
