using Microsoft.AspNetCore.Mvc;
using Safi.Dto.EmergencyDto;
using Safi.Interfaces;

namespace Safi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmergencyController : ControllerBase
    {
        private readonly IEmergency _repo;

        public EmergencyController(IEmergency repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var emergenciesDto = await _repo.GetAllAsync();
            return Ok(emergenciesDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var emergencyDto = await _repo.GetByIdAsync(id);
            if (emergencyDto == null) return NotFound();
            return Ok(emergencyDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmergencyDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var emergencyDto = await _repo.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = emergencyDto.Id }, emergencyDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEmergencyDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var emergencyDto = await _repo.UpdateAsync(id, dto);
            if (emergencyDto == null) return NotFound();

            return Ok(emergencyDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("department/{departmentId}")]
        public async Task<IActionResult> GetByDepartmentId(int departmentId)
        {
            var emergenciesDto = await _repo.GetByDepartmentIdAsync(departmentId);
            return Ok(emergenciesDto);
        }

        [HttpGet("department/name/{departmentName}")]
        public async Task<IActionResult> GetByDepartmentName(string departmentName)
        {
            var emergenciesDto = await _repo.GetByDepartmentNameAsync(departmentName);
            return Ok(emergenciesDto);
        }

        [HttpGet("number/{roomNumber}")]
        public async Task<IActionResult> GetByRoomNumber(int roomNumber)
        {
            var emergenciesDto = await _repo.GetByRoomNumberAsync(roomNumber);
            return Ok(emergenciesDto);
        }

        [HttpGet("check-unique/{roomNumber}/department/{departmentId}")]
        public async Task<IActionResult> IsRoomNumberUnique(int roomNumber, int departmentId)
        {
            var isUnique = await _repo.IsRoomNumberUniqueAsync(roomNumber, departmentId);
            return Ok(new { isUnique });
        }
    }
}
