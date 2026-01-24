using Safi.Dto.ICUDto;
using Safi.Models;

namespace Safi.Interfaces
{
    public interface IICU
    {
        Task<List<ICUDto>> GetAllAsync();
        Task<ICUDto?> GetByIdAsync(int id);
        Task<ICUDto> CreateAsync(CreateICUDto dto);
        Task<ICUDto?> UpdateAsync(int id, UpdateICUDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<ICUDto>> GetByDepartmentIdAsync(int departmentId);
        Task<List<ICUDto>> GetByDepartmentNameAsync(string departmentName);
        Task<List<ICUDto>> GetByRoomNumberAsync(int roomNumber);
        Task<bool> IsRoomNumberUniqueAsync(int roomNumber, int departmentId, int? excludeId = null);
    }
}
