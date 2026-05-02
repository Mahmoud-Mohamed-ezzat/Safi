using Safi.Dto.AssignWorksDto;
using Safi.Models;

namespace Safi.Interfaces
{
    public interface IAssignWorks
    {
        Task<List<AssignWorksDto>> GetAllAsync();
        Task<AssignWorksDto?> GetByIdAsync(int id);
        Task<AssignWorksDto> CreateAsync(CreateAssignWorksDto dto);
        Task<AssignWorksDto?> UpdateAsync(int id, UpdateAssignWorksDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<AssignWorksDto>> GetByDoctorIdAsync(string doctorId);
        Task<List<AssignWorksDto>> GetByRoomIdAsync(int roomId);
        Task<List<AssignWorksDto>> GetByDateAsync(DateOnly date);
        Task<List<AssignWorksDto>> GetByDateAndDoctorIdAsync(DateOnly date, string doctorId);
        Task<List<AssignWorksDto>> GetAssignWorksForAttendanceByDate(DateOnly date);
        Task<List<AssignWorksDto>> GetAssignWorksForAttendancetoday();
        Task<bool> IsRoomAssignedToSameDoctorAsync(int roomId, string doctorId);
        Task<bool> IsRoomExistAsync(int roomId);
    }
}
