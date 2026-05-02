using Safi.Dto.Attendance;

namespace Safi.Interfaces
{
    public interface IAttendance
    {
        public Task<List<GetAttendanceDto>> GetAllAsync();
        public Task<GetAttendanceDto?> GetByIdAsync(int id);
        public Task<List<GetAttendanceDto?>> GetByIdOfDoctorAsync(string id);
        public Task<List<GetAttendanceDto?>> GetByIdAndDateOfDoctorAsync(string id,DateOnly Day);
        public Task<List<GetAttendanceDto?>> GetAllByDateAsync(DateOnly Day);
        public Task<List<GetAttendanceDto?>> GetAllTodayAsync();
        public Task<GetAttendanceDto> CreateAttendanceAsync(CreateAttendanceDto dto);
        public Task<GetAttendanceDto> UpdateAttendanceAsync(int id, UpdateAttendanceDto dto);
        public Task<bool> DeleteAttendanceAsync(int id);
    }
}
