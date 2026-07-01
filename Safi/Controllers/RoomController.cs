using Microsoft.AspNetCore.Mvc;
using Safi.Dto.RoomDto;
using Safi.Interfaces;
using Microsoft.AspNetCore.Authorization;
namespace Safi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoom _repo;

        public RoomController(IRoom repo)
        {
            _repo = repo;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roomsDto = await _repo.GetAllAsync();
            return Ok(roomsDto);
        }
        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetAllRoomsByType(string type)
        {
            var roomsDto = await _repo.GetAllRoomsByTypeAsync(type);
            return Ok(roomsDto);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var roomDto = await _repo.GetByIdAsync(id);
            if (roomDto == null) return NotFound();
            return Ok(roomDto);
        }
        [Authorize(Roles = "Admin,subadmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoomDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var roomDto = await _repo.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = roomDto.Id }, roomDto);
        }
    [Authorize(Roles = "Admin,subadmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var roomDto = await _repo.UpdateAsync(id, dto);
            if (roomDto == null) return NotFound();

            return Ok(roomDto);
        }
    [Authorize(Roles = "Admin,subadmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
        [Authorize]
        [HttpGet("department/{departmentId}")]
        public async Task<IActionResult> GetByDepartmentId(int departmentId)
        {
            var roomsDto = await _repo.GetByDepartmentIdAsync(departmentId);
            return Ok(roomsDto);
        }
        [HttpGet("department/{departmentId}/type/")]
        public async Task<IActionResult> GetRoomByDepartmentIdAsyncandtype(int departmentId)
        {
            var roomsDto = await _repo.GetRoomByDepartmentIdAsyncandtype(departmentId);
            if (roomsDto == null) return BadRequest("No rooms found for this department");
            if (roomsDto.Count() == 0) return BadRequest("No rooms found for this department");
            return Ok(roomsDto);
        }
        [Authorize]
        [HttpGet("department/name/{departmentName}")]
        public async Task<IActionResult> GetByDepartmentName(string departmentName)
        {
            var roomsDto = await _repo.GetByDepartmentNameAsync(departmentName);
            return Ok(roomsDto);
        }
        [Authorize]
        [HttpGet("number/{roomNumber}")]
        public async Task<IActionResult> GetByRoomNumber(int roomNumber)
        {
            var roomsDto = await _repo.GetByRoomNumberAsync(roomNumber);
            return Ok(roomsDto);
        }

        [HttpGet("check-unique/{roomNumber}/department/{departmentId}")]
        public async Task<IActionResult> IsRoomNumberUnique(int roomNumber, int departmentId)
        {
            var isUnique = await _repo.IsRoomNumberUniqueAsync(roomNumber, departmentId);
            return Ok(new { isUnique });
        }
    }
}
