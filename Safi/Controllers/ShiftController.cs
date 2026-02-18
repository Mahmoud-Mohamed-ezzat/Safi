using Microsoft.AspNetCore.Mvc;
using Safi.Dto.ShiftDto;
using Safi.Dto.Account;
using Safi.Dto.AssignRoomToDoctorDto;
using Safi.Interfaces;

namespace Safi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly IShift _shiftRepo;

        public ShiftController(IShift shiftRepo)
        {
            _shiftRepo = shiftRepo;
        }

        // CRUD
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var shifts = await _shiftRepo.GetAllAsync();
            return Ok(shifts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var shift = await _shiftRepo.GetByIdAsync(id);
            if (shift == null) return NotFound("Shift not found");
            return Ok(shift);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShiftDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var shift = await _shiftRepo.CreateAsync(dto);
            if (shift == null) return BadRequest("Shift not created");
            return CreatedAtAction(nameof(GetById), new { id = shift.Id }, shift);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateShiftDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var shift = await _shiftRepo.UpdateAsync(id, dto);
            if (shift == null) return BadRequest("Shift not updated");
            return Ok(shift);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _shiftRepo.DeleteAsync(id);
            if (!deleted) return BadRequest("Shift not deleted");
            return NoContent();
        }

        // Specific Retrieval
        [HttpGet("{shiftId}/doctors")]
        public async Task<IActionResult> GetDoctorsByShift(int shiftId)
        {
            var doctors = await _shiftRepo.GetDoctorsByShiftIdAsync(shiftId);
            if (doctors == null) return BadRequest("Doctors not found in this shift");
            return Ok(doctors);
        }

        [HttpGet("{shiftId}/doctors/{doctorId}")]
        public async Task<IActionResult> GetDoctorByShift(int shiftId, string doctorId)
        {
            var doctor = await _shiftRepo.GetDoctorByShiftIdAsync(shiftId, doctorId);
            if (doctor == null) return BadRequest("Doctor not found in this shift");
            return Ok(doctor);
        }

        [HttpGet("{shiftId}/assignments")]
        public async Task<IActionResult> GetAssignmentsByShift(int shiftId)
        {
            var assignments = await _shiftRepo.GetAssignmentsByShiftIdAsync(shiftId);
            if (assignments == null) return BadRequest("Assignments not found in this shift");
            return Ok(assignments);
        }

        [HttpGet("{shiftId}/assignments/room/{roomId}")]
        public async Task<IActionResult> GetAssignmentByShiftAndRoom(int shiftId, int roomId)
        {
            var assignment = await _shiftRepo.GetAssignmentByShiftIdAndRoomIdAsync(shiftId, roomId);
            if (assignment == null) return BadRequest("No assignment found for this room in this shift");
            return Ok(assignment);
        }
    }
}
