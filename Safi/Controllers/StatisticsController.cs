using Microsoft.AspNetCore.Mvc;
using Safi.Dto.Statistics;
using Safi.Interfaces;

namespace Safi.Controllers
{
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
    }
}
