using Safi.Dto.AvailableTimeOFDoctor;

namespace Safi.Interfaces
{
    public interface IAvailableTimeOfDoctor
    {
        Task<AvailableTimeInfoDto> CreateAvailableTime(CreateAvailableTimeDto dto);
        Task<AvailableTimeInfoDto?> GetAvailableTimeById(int id);
        Task<List<AvailableTimeInfoDto>> GetAllAvailableTimes();
        Task<AvailableTimeInfoDto?> UpdateAvailableTimebyDoctor(int id, UpdateAvailableTimeDto2 dto);
        Task<AvailableTimeInfoDto?> UpdateAvailableTimebyreceptionist(int id, UpdateAvailableTimeDto dto);
        Task<List<AvailableTimeInfoDto>> GetAvailableTimesByDoctorId(string doctorId);
        Task<List<AvailableTimeInfoDto>> GetAvailableTimesByDoctorName(string doctorName);
        Task<List<AvailableTimeInfoDto>> GetAvailableTimesByDate(DateOnly day);
        Task<List<AvailableTimeInfoDto>> GetAvailableTimesByDepartment(string Department);
        Task<bool> DeleteAvailableTime(int id);
        Task<List<AvailableTimeInfoDto>> GetAvailableTimesByDoctorIdAndDate(string doctorId, DateOnly day);
        Task<List<AvailableTimeInfoDto>> GetAvailableTimesOfDoctorByDateandTime(string doctorId, DateOnly day, TimeOnly time);
        Task<List<AvailableTimeInfoDto>> GetAllAvailableTimesByDateandTime(DateOnly day, TimeOnly time);

    }
}
