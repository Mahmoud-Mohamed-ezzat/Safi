using Safi.Dto.Statistics;

namespace Safi.Interfaces
{
    public interface IStatisticsRepo
    {
        Task<GeneralStatsDto> GetGeneralStatsAsync();
        Task<List<DepartmentPatientStatsDto>> GetPatientsPerDepartmentAsync();
        Task<PatientDistributionDto> GetPatientDistributionAsync();
        Task<List<DepartmentRoomStatsDto>> GetRoomStatsPerDepartmentAsync();
        Task<SharedRoomStatsDto> GetSharedRoomStatsAsync();
        Task<List<ResponsibleDoctorDto>> GetResponsibleDoctorsForPatientAsync(string patientId);
        Task<int> GetNumberofHeartICUs();
        Task<int> GetNumberofKidneyICUs();
        Task<int> GetNumberofLiverICus();
        Task<int> GetNumberofHeartRooms();
        Task<int> GetNumberofKidneyRooms();
        Task<int> GetNumberofLiverRooms();
        Task<int> GetNumberofHeartEmergencies();
        Task<int> GetNumberofKidneyEmergencies();
        Task<int> GetNumberofLiverEmergencies();


    }
}
