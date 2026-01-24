using Microsoft.AspNetCore.Mvc;
using Safi.Dto.ICUDto;
using Safi.Interfaces;

namespace Safi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ICUController : ControllerBase
    {
        private readonly IICU _repo;

        public ICUController(IICU repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var icusDto = await _repo.GetAllAsync();
            return Ok(icusDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var icuDto = await _repo.GetByIdAsync(id);
            if (icuDto == null) return NotFound();
            return Ok(icuDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateICUDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var icuDto = await _repo.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = icuDto.Id }, icuDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateICUDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var icuDto = await _repo.UpdateAsync(id, dto);
            if (icuDto == null) return NotFound();

            return Ok(icuDto);
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
            var icusDto = await _repo.GetByDepartmentIdAsync(departmentId);
            return Ok(icusDto);
        }

        [HttpGet("department/name/{departmentName}")]
        public async Task<IActionResult> GetByDepartmentName(string departmentName)
        {
            var icusDto = await _repo.GetByDepartmentNameAsync(departmentName);
            return Ok(icusDto);
        }

        [HttpGet("number/{roomNumber}")]
        public async Task<IActionResult> GetByRoomNumber(int roomNumber)
        {
            var icusDto = await _repo.GetByRoomNumberAsync(roomNumber);
            return Ok(icusDto);
        }

        [HttpGet("check-unique/{roomNumber}/department/{departmentId}")]
        public async Task<IActionResult> IsRoomNumberUnique(int roomNumber, int departmentId)
        {
            var isUnique = await _repo.IsRoomNumberUniqueAsync(roomNumber, departmentId);
            return Ok(new { isUnique });
        }
    }
}
