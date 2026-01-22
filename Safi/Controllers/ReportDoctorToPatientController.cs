using Microsoft.AspNetCore.Mvc;
using Safi.Dto.ReportDoctorToPatientDto;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;

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
            var dtos = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var report = await _repo.GetByIdAsync(id);
            if (report == null)
            {
                return NotFound();
            }
            return Ok(report.ToReportDoctorToPatientDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReportDoctorToPatientDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var report = await _repo.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = report.Id }, report.ToReportDoctorToPatientDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateReportDoctorToPatientDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var report = await _repo.UpdateAsync(id, dto);
            if (report == null)
            {
                return NotFound();
            }

            return Ok(report.ToReportDoctorToPatientDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var success = await _repo.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatientId([FromRoute] string patientId)
        {
            var reports = await _repo.GetByPatientIdAsync(patientId);
            var dtos = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(dtos);
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetByDoctorId([FromRoute] string doctorId)
        {
            var reports = await _repo.GetByDoctorIdAsync(doctorId);
            var dtos = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(dtos);
        }

        [HttpGet("doctor/{doctorId}/date/{date}")]
        public async Task<IActionResult> GetByDoctorIdAndDate(string doctorId, DateOnly date)
        {
            var reports = await _repo.GetByDoctorIdAndDateAsync(doctorId, date);
            var dtos = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(dtos);
        }

        [HttpGet("search/medicine")]
        public async Task<IActionResult> GetByMedicineAndPatient([FromBody] string medicine, [FromBody] string patientId)
        {
            var reports = await _repo.GetByMedicineAndPatientAsync(medicine, patientId);
            var dtos = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(dtos);
        }

        [HttpGet("search/patient-name")]
        public async Task<IActionResult> GetByPatientName([FromBody] string patientName)
        {
            var reports = await _repo.GetByPatientNameAsync(patientName);
            var dtos = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(dtos);
        }

        [HttpGet("search/doctor-name")]
        public async Task<IActionResult> GetByDoctorName([FromBody] string doctorName)
        {
            var reports = await _repo.GetByDoctorNameAsync(doctorName);
            var dtos = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(dtos);
        }

        [HttpGet("search/doctor-patient")]
        public async Task<IActionResult> GetByDoctorNameAndPatientName([FromBody] string doctorName, [FromBody] string patientName)
        {
            var reports = await _repo.GetByDoctorNameandPatientNameAsync(doctorName, patientName);
            var dtos = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(dtos);
        }
        [HttpGet("date/{date}/patient/name/{patientName}")]
        public async Task<IActionResult> GetByDateAndPatientName(DateOnly date, string patientName)
        {
            var reports = await _repo.GetByDateAsyncandNameOfPatient(date, patientName);
            var reportsDto = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(reportsDto);
        }
        [HttpGet("date/{date}/doctor/name/{doctorName}")]
        public async Task<IActionResult> GetByDateAndDoctorName(DateOnly date, string doctorName)
        {
            var reports = await _repo.GetByDateAsyncandNameOfDoctor(date, doctorName);
            var reportsDto = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(reportsDto);
        }
    }
}
