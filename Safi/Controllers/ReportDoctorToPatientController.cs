using Microsoft.AspNetCore.Mvc;
using Safi.Dto.EmailDto;
using Safi.Dto.ReportDoctorToPatientDto;
using Safi.Interfaces;
using Safi.Mapper;
using Microsoft.AspNetCore.Identity;
using Safi.Models;

namespace Safi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportDoctorToPatientController : ControllerBase
    {
        private readonly IReportDoctorToPatient _repo;
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;

        public ReportDoctorToPatientController(IReportDoctorToPatient repo, IEmailService emailService)
        {
            _repo = repo;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reports = await _repo.GetAllAsync();
            var dtos = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
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
            if (report == null)
            {
                return NotFound();
            }
            await _userManager.SetEmailAsync(report.Patient, report.Patient.Email);
            // Send email notification to patient if medicines were prescribed
            if (report.Patient != null && !string.IsNullOrEmpty(report.Patient.Email) &&
                report.Medicines != null && report.Medicines.Any())
            {
                var medicinesList = string.Join("</li><li>", report.Medicines);
                await _emailService.SendEmailAsync(new SendEmailDto
                {
                    ToEmail = report.Patient.Email,
                    Subject = "New Prescription from Your Doctor",
                    Body = $@"
                        <h2>New Prescription Notification</h2>
                        <p>Dear {report.Patient.Name},</p>
                        <p>Dr. {report.Doctor?.Name ?? "Your doctor"} has prescribed new medicines for you.</p>
                        <p><strong>Prescribed Medicines:</strong></p>
                        <ul>
                            <li>{medicinesList}</li>
                        </ul>
                        <p><strong>Report Details:</strong></p>
                        <p>{report.Report}</p>
                        <p><strong>Date:</strong> {report.CreatedAt:MMMM dd, yyyy}</p>
                        <p>Please consult with your doctor if you have any questions about your prescription.</p>
                        <p>Best regards,<br/>Safi Hospital Team</p>"
                });
            }

            return CreatedAtAction(nameof(GetById), new { id = report.Id }, report.ToReportDoctorToPatientDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReportDoctorToPatientDto dto)
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
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _repo.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatientId(string patientId)
        {
            var reports = await _repo.GetByPatientIdAsync(patientId);
            var dtos = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(dtos);
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetByDoctorId(string doctorId)
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

        [HttpGet("patient/{patientId}/medicine/{medicine}")]
        public async Task<IActionResult> GetByMedicineAndPatient(string medicine, string patientId)
        {
            var reports = await _repo.GetByMedicineAndPatientAsync(medicine, patientId);
            var dtos = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(dtos);
        }

        [HttpGet("patient/name/{patientName}")]
        public async Task<IActionResult> GetByPatientName(string patientName)
        {
            var reports = await _repo.GetByPatientNameAsync(patientName);
            var dtos = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(dtos);
        }

        [HttpGet("doctor/name/{doctorName}")]
        public async Task<IActionResult> GetByDoctorName(string doctorName)
        {
            var reports = await _repo.GetByDoctorNameAsync(doctorName);
            var dtos = reports.Select(r => r.ToReportDoctorToPatientDto());
            return Ok(dtos);
        }

        [HttpGet("doctor/name/{doctorName}/patient/name/{patientName}")]
        public async Task<IActionResult> GetByDoctorNameAndPatientName(string doctorName, string patientName)
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
