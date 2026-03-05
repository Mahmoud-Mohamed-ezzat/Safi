using Microsoft.AspNetCore.Mvc;
using Safi.Dto.AppointmentToRoom;
using Safi.Dto.ReportDoctorToPatientDto;
using Safi.Interfaces;

namespace Safi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentToRoomController : ControllerBase
    {
        private readonly IAppointmentToRoom _repo;

        public AppointmentToRoomController(IAppointmentToRoom repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _repo.GetAllAsync();
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _repo.GetByIdAsync(id);
            if (appointment == null) return NotFound();
            return Ok(appointment);
        }

        [HttpPut("{id}/end-time")]
        public async Task<IActionResult> UpdateEndTime(int id, [FromBody] CreateReportWhenPatientGetOutRoomDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.id) return BadRequest("ID mismatch");

            var appointment = await _repo.UpdateEndTimeAsync(dto);
            if (appointment == null) return NotFound();

            return Ok(appointment);
        }

        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetByRoomId(int roomId)
        {
            var appointments = await _repo.GetByRoomIdAsync(roomId);
            return Ok(appointments);
        }

        [HttpGet("room/{roomId}/active")]
        public async Task<IActionResult> GetActiveByRoomId(int roomId)
        {
            var appointment = await _repo.GetActiveAppointmentByRoomIdAsync(roomId);
            if (appointment == null) return NotFound();
            return Ok(appointment);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatientId(string patientId)
        {
            var appointments = await _repo.GetByPatientIdAsync(patientId);
            return Ok(appointments);
        }

        [HttpGet("patient/{patientId}/date/{date}")]
        public async Task<IActionResult> GetByPatientIdAndDate(string patientId, DateOnly date)
        {
            var appointments = await _repo.GetByPatientIdandDateAsync(patientId, date);
            return Ok(appointments);
        }

        [HttpGet("patient/name/{patientName}")]
        public async Task<IActionResult> GetByPatientName(string patientName)
        {
            var appointments = await _repo.GetByPatientNameAsync(patientName);
            return Ok(appointments);
        }

        [HttpGet("patient/name/{patientName}/date/{date}")]
        public async Task<IActionResult> GetByPatientNameAndDate(string patientName, DateOnly date)
        {
            var appointments = await _repo.GetByPatientNameandDateAsync(patientName, date);
            return Ok(appointments);
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetByPrimaryDoctorId(string doctorId)
        {
            var appointments = await _repo.GetByPrimaryDoctorIdAsync(doctorId);
            return Ok(appointments);
        }

        [HttpGet("doctor/{doctorId}/date/{date}")]
        public async Task<IActionResult> GetByDoctorIdAndDate(string doctorId, DateOnly date)
        {
            var appointments = await _repo.GetByPrimaryDoctorIdandDateAsync(doctorId, date);
            return Ok(appointments);
        }

        [HttpGet("doctor/{doctorId}/all")]
        public async Task<IActionResult> GetAllAppointmentsForDoctor(string doctorId)
        {
            var appointments = await _repo.GetAllAppointmentofDoctorasprimaryorNot(doctorId);
            return Ok(appointments);
        }

        [HttpGet("doctor/name/{doctorName}")]
        public async Task<IActionResult> GetByDoctorName(string doctorName)
        {
            var appointments = await _repo.GetByDoctorNameAsync(doctorName);
            return Ok(appointments);
        }

        [HttpGet("doctor/name/{doctorName}/date/{date}")]
        public async Task<IActionResult> GetByDoctorNameAndDate(string doctorName, DateOnly date)
        {
            var appointments = await _repo.GetByDoctorNameandDateAsync(doctorName, date);
            return Ok(appointments);
        }

        [HttpGet("room/{roomId}/date/{date}")]
        public async Task<IActionResult> GetByRoomIdAndDate(int roomId, DateOnly date)
        {
            var appointments = await _repo.GetByRoomIdandDateAsync(roomId, date);
            return Ok(appointments);
        }

        [HttpGet("{appointmentId}/doctors")]
        public async Task<IActionResult> GetDoctorsInAppointment(int appointmentId)
        {
            var doctors = await _repo.GetAllDoctorsinthisRoomInthisAppointment(appointmentId);
            return Ok(doctors);
        }
        [HttpGet("{doctorId}/patients")]
        public async Task<IActionResult> GetPatientsInDoctor(string doctorId)
        {
            var patients = await _repo.GetallpatientsdealwithDoctor(doctorId);
            return Ok(patients);
        }
    }
}
