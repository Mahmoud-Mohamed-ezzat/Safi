using Safi.Dto.ReportDoctorToPatientDto;
using Safi.Models;

namespace Safi.Mapper
{
    public static class ReportDoctorToPatientMapper
    {
        public static ReportDoctorToPatientDto ToReportDoctorToPatientDto(this ReportDoctorToPatient report)
        {
            return new ReportDoctorToPatientDto
            {
                Id = report.Id,
                PatientId = report.PatientId,
                PatientName = report.Patient?.Name,
                DoctorId = report.DoctorId,
                DoctorName = report.Doctor?.Name,
                Report = report.Report,
                CreatedAt = report.CreatedAt,
                Medicines = report.Medicines
            };
        }

        public static ReportDoctorToPatient ToReportDoctorToPatient(this CreateReportDoctorToPatientDto dto)
        {
            return new ReportDoctorToPatient
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                Report = dto.Report,
                Medicines = dto.Medicines ?? new List<string>()
            };
        }
    }
}
