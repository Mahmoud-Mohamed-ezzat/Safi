using Safi.Dto.ReportDoctorToPatientDto;
using Safi.Models;

namespace Safi.Mapper
{
    public static class ReportDoctorToPatientMapper
    {
        public static ReportDoctorToPatient ToReportDoctorToPatient(this CreateReportDoctorToPatientDto createReportDto)
        {
            return new ReportDoctorToPatient
            {
                PatientId = createReportDto.PatientId,
                DoctorId = createReportDto.DoctorId,
                Report = createReportDto.Report,
                Medicines = createReportDto.Medicines
            };
        }

        public static ReportDoctorToPatientDto ToReportDoctorToPatientDto(this ReportDoctorToPatient report)
        {
            return new ReportDoctorToPatientDto
            {
                Id = report.Id,
                PatientId = report.PatientId,
                PatientName = report.Patient?.Name ?? "Unknown", // Assuming Patient has Name
                DoctorId = report.DoctorId,
                DoctorName = report.Doctor?.Name ?? "Unknown", // Assuming Doctor has Name
                Report = report.Report,
                Medicines = report.Medicines,
                CreatedAt = report.CreatedAt
            };
        }
    }
}
