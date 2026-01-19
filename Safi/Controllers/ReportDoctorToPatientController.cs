using Microsoft.AspNetCore.Mvc;
using Safi.Dto.ReportDoctorToPatientDto;
using Safi.Interfaces;
using Safi.Mapper;

namespace Safi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportDoctorToPatientController : ControllerBase
    {
        private readonly IReportDoctorToPatient _repo;

        public ReportDoctorToPatientController(IReportDoctorToPatient repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reports = await _repo.GetAllAsync();
            var reportsDto = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(reportsDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var report = await _repo.GetByIdAsync(id);
            if (report == null) return NotFound();
            return Ok(report.ToReportDoctorToPatientDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReportDoctorToPatientDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var report = await _repo.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = report.Id }, report.ToReportDoctorToPatientDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReportDoctorToPatientDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var report = await _repo.UpdateAsync(id, dto);
            if (report == null) return NotFound();

            return Ok(report.ToReportDoctorToPatientDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatientId(string patientId)
        {
            var reports = await _repo.GetByPatientIdAsync(patientId);
            var reportsDto = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(reportsDto);
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetByDoctorId(string doctorId)
        {
            var reports = await _repo.GetByDoctorIdAsync(doctorId);
            var reportsDto = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(reportsDto);
        }

        [HttpGet("doctor/{doctorId}/date/{date}")]
        public async Task<IActionResult> GetByDoctorIdAndDate(string doctorId, DateTime date)
        {
            var reports = await _repo.GetByDoctorIdAndDateAsync(doctorId, date);
            var reportsDto = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(reportsDto);
        }

        [HttpGet("patient/{patientId}/medicine/{medicine}")]
        public async Task<IActionResult> GetByMedicineAndPatient(string medicine, string patientId)
        {
            var reports = await _repo.GetByMedicineAndPatientAsync(medicine, patientId);
            var reportsDto = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(reportsDto);
        }

        [HttpGet("patient/name/{name}")]
        public async Task<IActionResult> GetByPatientName(string name)
        {
            var reports = await _repo.GetByPatientNameAsync(name);
            var reportsDto = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(reportsDto);
        }

        [HttpGet("doctor/name/{name}")]
        public async Task<IActionResult> GetByDoctorName(string name)
        {
            var reports = await _repo.GetByDoctorNameAsync(name);
            var reportsDto = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(reportsDto);
        }

        [HttpGet("doctor/name/{doctorName}/patient/name/{patientName}")]
        public async Task<IActionResult> GetByDoctorNameAndPatientName(string doctorName, string patientName)
        {
            var reports = await _repo.GetByDoctorNameandPatientNameAsync(doctorName, patientName);
            var reportsDto = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(reportsDto);
        }
    }
}
