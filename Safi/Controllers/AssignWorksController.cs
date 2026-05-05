using Microsoft.AspNetCore.Mvc;
using Safi.Dto.AssignWorksDto;
using Safi.Interfaces;

namespace Safi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignWorksController : ControllerBase
    {
        private readonly IAssignWorks _assignWorksRepo;

        public AssignWorksController(IAssignWorks assignWorksRepo)
        {
            _assignWorksRepo = assignWorksRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var assignments = await _assignWorksRepo.GetAllAsync();
            return Ok(assignments);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var assignment = await _assignWorksRepo.GetByIdAsync(id);
            if (assignment == null) return NotFound();
            return Ok(assignment);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAssignWorksDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var assignment = await _assignWorksRepo.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = assignment.Id }, assignment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAssignWorksDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var assignment = await _assignWorksRepo.UpdateAsync(id, dto);
                if (assignment == null) return NotFound();
                return Ok(assignment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _assignWorksRepo.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetByDoctorId(string doctorId)
        {
            var assignments = await _assignWorksRepo.GetByDoctorIdAsync(doctorId);
            return Ok(assignments);
        }

        [HttpGet("room/{roomId:int}")]
        public async Task<IActionResult> GetByRoomId(int roomId)
        {
            var assignments = await _assignWorksRepo.GetByRoomIdAsync(roomId);
            return Ok(assignments);
        }

        [HttpGet("date/{date}")]
        public async Task<IActionResult> GetByDate(DateOnly date)
        {
            var assignments = await _assignWorksRepo.GetByDateAsync(date);
            return Ok(assignments);
        }



        [HttpGet("GetAssignWorksForAttendancetoday")]
        public async Task<IActionResult> GetAssignWorksForAttendancetoday()
        {
            var assignments = await _assignWorksRepo.GetAssignWorksForAttendancetoday();
            return Ok(assignments);
        }
        [HttpGet("GetAssignWorksForAttendanceBydate/{date}")]
        public async Task<IActionResult> GetAssignWorksForAttendanceBydate(DateOnly date)
        {
            var assignments = await _assignWorksRepo.GetAssignWorksForAttendanceByDate(date);
            return Ok(assignments);
        }
    }
}
