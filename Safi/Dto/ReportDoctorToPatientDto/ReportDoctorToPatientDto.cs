using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.ReportDoctorToPatientDto
{
    public class ReportDoctorToPatientDto
    {
        public int Id { get; set; }
        public string PatientId { get; set; }
        public string? PatientName { get; set; }
        public string DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public string Report { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string>? Medicines { get; set; }
    }
}
