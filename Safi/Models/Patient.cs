using System.ComponentModel.DataAnnotations.Schema;

namespace Safi.Models
{
    public class Patient : User
    {
        public bool? HasSugar { get; set; } // Diabetes indicator
        public bool? HasPressure { get; set; } // Hypertension indicator
        public string? History { get; set; }
        // Navigation properties
        public virtual ICollection<Bill>? Bills { get; set; } = new HashSet<Bill>();
        public virtual ICollection<Department>? Departments { get; set; } = new HashSet<Department>();
        public virtual ICollection<AppointmentToRoom>? Appointments { get; set; } = new List<AppointmentToRoom>();
        public virtual ICollection<ReportDoctorToPatient>? Reports { get; set; } = new List<ReportDoctorToPatient>();
        public virtual ICollection<Reservation>? Reservations { get; set; } = new HashSet<Reservation>();
        public virtual ICollection<Analysis>? Analyses { get; set; } = new HashSet<Analysis>();

    }
}
