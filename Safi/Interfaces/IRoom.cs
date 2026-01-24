using Safi.Dto.RoomDto;
using Safi.Models;

namespace Safi.Interfaces
{
    public interface IRoom
    {
        Task<List<RoomDto>> GetAllAsync();
        Task<RoomDto?> GetByIdAsync(int id);
        Task<RoomDto> CreateAsync(CreateRoomDto dto);
        Task<RoomDto?> UpdateAsync(int id, UpdateRoomDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<RoomDto>> GetByDepartmentIdAsync(int departmentId);
        Task<List<RoomDto>> GetByDepartmentNameAsync(string departmentName);
        Task<List<RoomDto>> GetByRoomNumberAsync(int roomNumber);
        Task<bool> IsRoomNumberUniqueAsync(int roomNumber, int departmentId, int? excludeId = null);
    }
}
