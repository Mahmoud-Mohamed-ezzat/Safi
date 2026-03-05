using Microsoft.AspNetCore.Mvc;
using Safi.Dto.Reservation;
using Safi.Interfaces;

namespace Safi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservation _repo;

        public ReservationController(IReservation repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reservations = await _repo.GetAllReservations();
            return Ok(reservations);
        }

        [HttpGet("unassigned")]
        public async Task<IActionResult> GetUnassigned()
        {
            var reservations = await _repo.GetAllUnassignedReservations();
            return Ok(reservations);
        }

        [HttpGet("assigned")]
        public async Task<IActionResult> GetAssigned()
        {
            var reservations = await _repo.GetAllAssignedReservations();
            return Ok(reservations);
        }

        [HttpDelete("{availableTimeId}")]
        public async Task<IActionResult> DeleteManyByAvailableTimeId(int availableTimeId)
        {
            var result = await _repo.DeleteManyReservationsByAvailableTimeId(availableTimeId);
            if (!result) return NotFound();
            return Ok(new { Message = "Reservations deleted successfully" });
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatientId(string patientId)
        {
            var reservations = await _repo.GetReservationByPatientId(patientId);
            return Ok(reservations);
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetByDoctorId(string doctorId)
        {
            var reservations = await _repo.GetReservationByDoctorId(doctorId);
            if (reservations == null) return NotFound();
            return Ok(reservations);
        }

        [HttpGet("patient/{patientId}/doctor/{doctorId}")]
        public async Task<IActionResult> GetByPatientIdAndDoctorId(string patientId, string doctorId)
        {
            var reservations = await _repo.GetReservationByPatientIdAndDoctorId(patientId, doctorId);
            if (reservations == null) return NotFound();
            return Ok(reservations);
        }

        [HttpGet("patient/{patientId}/date/{date}")]
        public async Task<IActionResult> GetByPatientIdAndDate(string patientId, DateTime date)
        {
            var reservations = await _repo.GetReservationByPatientIdAndDate(patientId, date);
            if (reservations == null) return NotFound();
            return Ok(reservations);
        }

        [HttpGet("doctor/{doctorId}/date/{date}")]
        public async Task<IActionResult> GetByDoctorIdAndDate(string doctorId, DateTime date)
        {
            var reservations = await _repo.GetReservationByDoctorIdAndDate(doctorId, date);
            if (reservations == null) return NotFound();
            return Ok(reservations);
        }

        [HttpGet("date/{date}")]
        public async Task<IActionResult> GetByDate(DateTime date)
        {
            var reservations = await _repo.GetReservationByDate(date);
            if (reservations == null) return NotFound();
            return Ok(reservations);
        }

        [HttpGet("doctor/{doctorId}/patients")]
        public async Task<IActionResult> GetPatientsByDoctorId(string doctorId)
        {
            var patients = await _repo.GetallpatientsdealwithDoctor(doctorId);
            if (patients == null) return NotFound();
            return Ok(patients);
        }

        [HttpPost("add-department")]
        public async Task<IActionResult> AddDepartmentToPatient([FromBody] addDepartmenttoPatientDepartmentWhenReservationIsCreatedDto dto)
        {
            var departmentName = await _repo.addDepartmenttoPatientDepartmentWhenReservationIsCreated(dto.PatientId, dto.DoctorId);
            if (departmentName == null) return NotFound();
            return Ok(new { DepartmentName = departmentName });
        }
    }
}
