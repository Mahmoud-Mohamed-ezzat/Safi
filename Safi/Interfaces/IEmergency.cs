using Safi.Dto.EmergencyDto;
using Safi.Models;

namespace Safi.Interfaces
{
    public interface IEmergency
    {
        Task<List<EmergencyDto>> GetAllAsync();
        Task<EmergencyDto?> GetByIdAsync(int id);
        Task<EmergencyDto> CreateAsync(CreateEmergencyDto dto);
        Task<EmergencyDto?> UpdateAsync(int id, UpdateEmergencyDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<EmergencyDto>> GetByDepartmentIdAsync(int departmentId);
        Task<List<EmergencyDto>> GetByDepartmentNameAsync(string departmentName);
        Task<List<EmergencyDto>> GetByRoomNumberAsync(int roomNumber);
        Task<bool> IsRoomNumberUniqueAsync(int roomNumber, int departmentId, int? excludeId = null);
    }
}
