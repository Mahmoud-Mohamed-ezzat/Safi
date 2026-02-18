using Safi.Dto.Account;
using Safi.Dto.ShiftDto;
using Safi.Dto.AssignRoomToDoctorDto;
namespace Safi.Interfaces
{
    public interface IShift
    {
        // CRUD
        Task<List<ShiftDto>> GetAllAsync();
        Task<ShiftDto?> GetByIdAsync(int id);
        Task<ShiftDto> CreateAsync(CreateShiftDto dto);
        Task<ShiftDto?> UpdateAsync(int id, UpdateShiftDto dto);
        Task<bool> DeleteAsync(int id);

        // Specific Retrieval
        Task<List<GetDoctorsDto>> GetDoctorsByShiftIdAsync(int shiftId);
        Task<List<GetDoctorsDto>> GetDoctorsAtDateByShiftIdAsync(int shiftId, DateOnly date);
        Task<List<GetDoctorsshiftDto>> GetDoctorsAtDateinroomByShiftIdAsync(int shiftId, DateOnly date, int roomId);
        Task<GetDoctorsDto?> GetDoctorByShiftIdAsync(int shiftId, string doctorId);
        Task<GetDoctorsDto?> GetDoctorAtDateByShiftIdAsync(int shiftId, DateOnly date, string doctorId);
        Task<GetDoctorsDto?> GetDoctorAtDateinroomByShiftIdAsync(int shiftId, DateOnly date, int roomId, string doctorId);
        Task<List<AssignRoomToDoctorDto>> GetAssignmentsByShiftIdAsync(int shiftId);
        Task<List<AssignRoomToDoctorDto>> GetAssignmentsAtDateByShiftIdAsync(int shiftId, DateOnly date);
        Task<List<AssignRoomToDoctorDto>> GetAssignmentsAtDateinroomByShiftIdAsync(int shiftId, DateOnly date, int roomId);
        Task<AssignRoomToDoctorDto?> GetAssignmentByShiftIdAndRoomIdAsync(int shiftId, int roomId);
    }
}
