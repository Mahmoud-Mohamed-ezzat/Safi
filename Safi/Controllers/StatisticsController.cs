using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Safi.Dto.Statistics;
using Safi.Interfaces;

namespace Safi.Controllers
{
    [Authorize(Roles ="Admin,SubAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsRepo _repo;

        public StatisticsController(IStatisticsRepo repo)
        {
            _repo = repo;
        }

        [HttpGet("general")]
        public async Task<ActionResult<GeneralStatsDto>> GetGeneralStats()
        {
            var stats = await _repo.GetGeneralStatsAsync();
            return Ok(stats);
        }

        [HttpGet("patients-per-department")]
        public async Task<ActionResult<List<DepartmentPatientStatsDto>>> GetPatientsPerDepartment()
        {
            return Ok(await _repo.GetPatientsPerDepartmentAsync());
        }

        [HttpGet("patient-distribution")]
        public async Task<ActionResult<PatientDistributionDto>> GetPatientDistribution()
        {
            return Ok(await _repo.GetPatientDistributionAsync());
        }

        [HttpGet("rooms-per-department")]
        public async Task<ActionResult<List<DepartmentRoomStatsDto>>> GetRoomStatsPerDepartment()
        {
            return Ok(await _repo.GetRoomStatsPerDepartmentAsync());
        }

        [HttpGet("shared-room-stats")]
        public async Task<ActionResult<SharedRoomStatsDto>> GetSharedRoomStats()
        {
            return Ok(await _repo.GetSharedRoomStatsAsync());
        }

        [HttpGet("responsible-doctors/{patientId}")]
        public async Task<ActionResult<List<ResponsibleDoctorDto>>> GetResponsibleDoctors(string patientId)
        {
            return Ok(await _repo.GetResponsibleDoctorsForPatientAsync(patientId));
        }

        [HttpGet("heart/icus")]
        public async Task<ActionResult<int>> GetHeartICUs() => Ok(await _repo.GetNumberofHeartICUs());

        [HttpGet("kidney/icus")]
        public async Task<ActionResult<int>> GetKidneyICUs() => Ok(await _repo.GetNumberofKidneyICUs());

        [HttpGet("liver/icus")]
        public async Task<ActionResult<int>> GetLiverICUs() => Ok(await _repo.GetNumberofLiverICus());

        [HttpGet("heart/rooms")]
        public async Task<ActionResult<int>> GetHeartRooms() => Ok(await _repo.GetNumberofHeartRooms());

        [HttpGet("kidney/rooms")]
        public async Task<ActionResult<int>> GetKidneyRooms() => Ok(await _repo.GetNumberofKidneyRooms());

        [HttpGet("liver/rooms")]
        public async Task<ActionResult<int>> GetLiverRooms() => Ok(await _repo.GetNumberofLiverRooms());

        [HttpGet("heart/emergencies")]
        public async Task<ActionResult<int>> GetHeartEmergencies() => Ok(await _repo.GetNumberofHeartEmergencies());

        [HttpGet("kidney/emergencies")]
        public async Task<ActionResult<int>> GetKidneyEmergencies() => Ok(await _repo.GetNumberofKidneyEmergencies());

        [HttpGet("liver/emergencies")]
        public async Task<ActionResult<int>> GetLiverEmergencies() => Ok(await _repo.GetNumberofLiverEmergencies());
    }
}
