using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.ReportDoctorToPatientDto
{
    public class CreateReportDoctorToPatientDto
    {
        [Required]
        public string PatientId { get; set; }
        [Required]
        public string DoctorId { get; set; }
        [Required]
        public string Report { get; set; }
        public List<string>? Medicines { get; set; } = new List<string>();
    }
}
