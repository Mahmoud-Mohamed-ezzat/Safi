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
    }
}
