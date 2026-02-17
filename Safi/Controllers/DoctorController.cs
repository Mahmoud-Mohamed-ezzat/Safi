using Microsoft.AspNetCore.Mvc;
using Safi.Dto.Account;
using Safi.Interfaces;

namespace Safi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctor _doctorRepo;

        public DoctorController(IDoctor doctorRepo)
        {
            _doctorRepo = doctorRepo;
        }

        [HttpPost("rate")]
        public async Task<IActionResult> RateDoctor([FromBody] RateDoctorDto rateDoctorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _doctorRepo.RateDoctorAsync(rateDoctorDto.DoctorId, rateDoctorDto.Rating);
            if (!result)
            {
                return NotFound("Doctor not found.");
            }

            return Ok("Rating updated successfully.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            return Ok(doctor);
        }
    }
}
