using Microsoft.AspNetCore.Mvc;
using Safi.Dto.AssignRoomToDoctorDto;
using Safi.Interfaces;

namespace Safi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignRoomToDoctorController : ControllerBase
    {
        private readonly IAssignRoomToDoctor _repo;

        public AssignRoomToDoctorController(IAssignRoomToDoctor repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var assignments = await _repo.GetAllAsync();
            return Ok(assignments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var assignment = await _repo.GetByIdAsync(id);
            if (assignment == null) return NotFound();
            return Ok(assignment);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAssignRoomToDoctorDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var assignment = await _repo.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = assignment.Id }, assignment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAssignRoomToDoctorDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var assignment = await _repo.UpdateAsync(id, dto);
            if (assignment == null) return NotFound();

            return Ok(assignment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetByDoctorId(string doctorId)
        {
            var assignments = await _repo.GetByDoctorIdAsync(doctorId);
            return Ok(assignments);
        }

        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetByRoomId(int roomId)
        {
            var assignments = await _repo.GetByRoomIdAsync(roomId);
            return Ok(assignments);
        }

        [HttpGet("date/{date}")]
        public async Task<IActionResult> GetByDate(DateOnly date)
        {
            var assignments = await _repo.GetByDateAsync(date);
            return Ok(assignments);
        }

        [HttpGet("date/{date}/doctor/{doctorId}")]
        public async Task<IActionResult> GetByDateAndDoctorId(DateOnly date, string doctorId)
        {
            var assignments = await _repo.GetByDateAndDoctorIdAsync(date, doctorId);
            return Ok(assignments);
        }

        [HttpGet("date/{date}/patient/{patientId}")]
        public async Task<IActionResult> GetByDateAndPatientId(DateOnly date, string patientId)
        {
            var assignments = await _repo.GetByDateAndPatientIdAsync(date, patientId);
            return Ok(assignments);
        }
    }
}
