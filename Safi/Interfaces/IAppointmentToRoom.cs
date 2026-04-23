using Safi.Dto.Account;
using Safi.Dto.AppointmentToRoom;
using Safi.Dto.ReportDoctorToPatientDto;
using Safi.Models;

namespace Safi.Interfaces
{
    public interface IAppointmentToRoom
    {
        Task<AppointmentToRoomDto> CreateAsync(CreateAppointmentToRoomDto dto);
        Task<AppointmentToRoomDto?> GetByIdAsync(int id);
        Task<List<AppointmentToRoomDto>> GetByRoomIdAsync(int roomId);
        Task<List<AppointmentToRoomDto>> GetByPatientIdAsync(string patientId);
        Task<List<AppointmentToRoomDto>> GetByPrimaryDoctorIdAsync(string doctorId);
        Task<List<AppointmentToRoomDto>> GetByPatientNameAsync(string PatientName);
        Task<List<AppointmentToRoomDto>> GetByDoctorNameAsync(string DoctorName);       
        Task<List<AppointmentToRoomDto>> GetByPatientIdandDateAsync(string patientId,DateOnly? Date);
        Task<List<AppointmentToRoomDto>> GetByPrimaryDoctorIdandDateAsync(string doctorId,DateOnly? Date);
        Task<List<AppointmentToRoomDto>> GetByRoomIdandDateAsync(int roomId,DateOnly? Date);
        Task<List<AppointmentToRoomDto>> GetByPatientNameandDateAsync(string PatientName,DateOnly? Date);
        Task<List<AppointmentToRoomDto>> GetByDoctorNameandDateAsync(string DoctorName,DateOnly? Date);
        Task<List<GetDoctorsDto>> GetAllDoctorsinthisRoomInthisAppointment(int AppointmentId);
        Task<List<AppointmentToRoomDto>> GetAllAppointmentofDoctorasprimaryorNot(string DoctorId);
        Task<List<GetPatientsDto>> GetallpatientsdealwithDoctor(string doctorId);
        Task<bool> DeleteAsync(int id);
        Task<AppointmentToRoomDto?> UpdateEndTimeAsync(CreateReportWhenPatientGetOutRoomDto dto);
        Task<AppointmentToRoomDto?> GetActiveAppointmentByRoomIdAsync(int roomId);
        Task<AppointmentToRoomDto?> GetActiveAppointmentByRoomIdAsync(int roomId,string doctorId);
        Task<List<AppointmentToRoomDto>> GetAllAsync();

    }
}
