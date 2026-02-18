using System.ComponentModel.DataAnnotations.Schema;

namespace Safi.Models
{
    public class Doctor : User
    {
        public string University { get; set; } // University graduated from
        public string Degree { get; set; } // Medical degree
        public float Rank { get; set; } // Ranking or rating
        public int RankCount { get; set; } // Number of ratings

        public int ?DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        // Navigation properties
        public virtual ICollection<AssignRoomToDoctor>? AssignRoomToDoctors { get; set; } = new List<AssignRoomToDoctor>();
        public virtual ICollection<ReportDoctorToPatient>? Reports { get; set; } = new List<ReportDoctorToPatient>();
        public virtual ICollection<TimeAvailableOfDoctor>? TimeAvailable { get; set; } = new List<TimeAvailableOfDoctor>();
        public virtual ICollection<Reservation>? Reservations { get; set; } = new List<Reservation>();
    }
}
