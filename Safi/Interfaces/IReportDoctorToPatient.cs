using Safi.Dto.ReportDoctorToPatientDto;
using Safi.Models;

namespace Safi.Interfaces
{
    public interface IReportDoctorToPatient
    {
        Task<List<ReportDoctorToPatient>> GetAllAsync();
        Task<ReportDoctorToPatient?> GetByIdAsync(int id);
        Task<ReportDoctorToPatient> CreateAsync(CreateReportDoctorToPatientDto dto);
        Task<ReportDoctorToPatient?> UpdateAsync(int id, UpdateReportDoctorToPatientDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<ReportDoctorToPatient>> GetByDateAsync(DateOnly date);
        Task<List<ReportDoctorToPatient>> GetByDateAsyncandNameOfPatient(DateOnly date,string patientName);
        Task<List<ReportDoctorToPatient>> GetByDateAsyncandNameOfDoctor(DateOnly date,string doctorName);
        Task<List<ReportDoctorToPatient>> GetByPatientIdAsync(string patientId);
        Task<List<ReportDoctorToPatient>> GetByDoctorIdAsync(string doctorId);
        Task<List<ReportDoctorToPatient>> GetByDoctorIdAndDateAsync(string doctorId, DateOnly date);
        Task<List<ReportDoctorToPatient>> GetByMedicineAndPatientAsync(string medicine, string patientId);
        Task<List<ReportDoctorToPatient>> GetByPatientNameAsync(string patientName);
        Task<List<ReportDoctorToPatient>> GetByDoctorNameAsync(string doctorName);
        Task<List<ReportDoctorToPatient>> GetByDoctorNameandPatientNameAsync(string doctorName,string patientName);
        Task<List<ReportDoctorToPatient>> GetAllReportwroteWhilePatientAppointsToRoom(string PatientId,int  AppointmentToRoomId);
    }
}