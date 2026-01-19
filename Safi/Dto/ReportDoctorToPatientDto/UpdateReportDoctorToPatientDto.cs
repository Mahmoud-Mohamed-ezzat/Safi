using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.ReportDoctorToPatientDto
{
    public class UpdateReportDoctorToPatientDto
    {
        [Required]
        public string Report { get; set; }
        public List<string>? Medicines { get; set; }
    }
}
