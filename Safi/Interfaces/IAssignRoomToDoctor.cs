using Safi.Dto.AssignRoomToDoctorDto;

namespace Safi.Interfaces
{
    public interface IAssignRoomToDoctor
    {
        Task<List<AssignRoomToDoctorDto>> GetAllAsync();
        Task<AssignRoomToDoctorDto?> GetByIdAsync(int id);
        Task<AssignRoomToDoctorDto> CreateAsync(CreateAssignRoomToDoctorDto dto);
        Task<AssignRoomToDoctorDto?> UpdateAsync(int id, UpdateAssignRoomToDoctorDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<AssignRoomToDoctorDto>> GetByDoctorIdAsync(string doctorId);
        Task<List<AssignRoomToDoctorDto>> GetByRoomIdAsync(int roomId);
        Task<List<AssignRoomToDoctorDto>> GetByDateAsync(DateOnly date);
        Task<List<AssignRoomToDoctorDto>> GetByDateAndDoctorIdAsync(DateOnly date, string doctorId);
        Task<List<AssignRoomToDoctorDto>> GetByDateAndPatientIdAsync(DateOnly date, string patientId);
        Task<bool> IsRoomAssignedToSameDoctorAsync(int roomId, string doctorId);
        Task<bool> IsRoomAvailableAsync(int roomId);
    }
}
