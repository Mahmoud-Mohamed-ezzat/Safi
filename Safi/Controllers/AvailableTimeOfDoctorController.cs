using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Safi.Dto.AvailableTimeOFDoctor;
using Safi.Interfaces;

namespace Safi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailableTimeOfDoctorController : ControllerBase
    {
        private readonly IAvailableTimeOfDoctor _availableTimeOfDoctor;
        public AvailableTimeOfDoctorController(IAvailableTimeOfDoctor availableTimeOfDoctor)
        {
            _availableTimeOfDoctor = availableTimeOfDoctor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAvailableTime(CreateAvailableTimeDto dto)
        {
            var availableTime = await _availableTimeOfDoctor.CreateAvailableTime(dto);
            return Ok(availableTime);
        }
        [HttpGet("GetAvailableTimesByDate")]
        public async Task<IActionResult> GetAvailableTimesByDate(DateOnly day)
        {
            var availableTimes = await _availableTimeOfDoctor.GetAvailableTimesByDate(day);
            return Ok(availableTimes);
        }
        [HttpGet("GetAvailableTimesByDoctorId")]
        public async Task<IActionResult> GetAvailableTimesByDoctorId(string doctorId)
        {
            var availableTimes = await _availableTimeOfDoctor.GetAvailableTimesByDoctorId(doctorId);
            return Ok(availableTimes);
        }
        [HttpGet("GetAvailableTimesByDoctorIdAndDate")]
        public async Task<IActionResult> GetAvailableTimesByDoctorIdAndDate(string doctorId, DateOnly day)
        {
            var availableTimes = await _availableTimeOfDoctor.GetAvailableTimesByDoctorIdAndDate(doctorId, day);
            return Ok(availableTimes);
        }
        [HttpGet("GetAvailableTimesByDoctorName")]
        public async Task<IActionResult> GetAvailableTimesByDoctorName(string doctorName)
        {
            var availableTimes = await _availableTimeOfDoctor.GetAvailableTimesByDoctorName(doctorName);
            return Ok(availableTimes);
        }
        [HttpGet("GetAvailableTimesByDepartment")]
        public async Task<IActionResult> GetAvailableTimesByDepartment(string department)
        {
            var availableTimes = await _availableTimeOfDoctor.GetAvailableTimesByDepartment(department);
            return Ok(availableTimes);
        }
        [HttpGet("GetAvailableTimesByDateandTime")]
        public async Task<IActionResult> GetAvailableTimesByDateandTime(DateOnly day, TimeOnly time)
        {
            var availableTimes = await _availableTimeOfDoctor.GetAllAvailableTimesByDateandTime(day, time);
            return Ok(availableTimes);
        }
        [HttpGet("GetAvailableTimesOfDoctorByDateandTime")]
        public async Task<IActionResult> GetAvailableTimesOfDoctorByDateandTime(string doctorId, DateOnly day, TimeOnly time)
        {
            var availableTimes = await _availableTimeOfDoctor.GetAvailableTimesOfDoctorByDateandTime(doctorId, day, time);
            return Ok(availableTimes);
        }
        [HttpDelete("DeleteAvailableTime")]
        public async Task<IActionResult> DeleteAvailableTime(int id)
        {
            var result = await _availableTimeOfDoctor.DeleteAvailableTime(id);
            return Ok(result);
        }
        [HttpPut("UpdateAvailableTimebyDoctor")]
        public async Task<IActionResult> UpdateAvailableTimebyDoctor(int id, UpdateAvailableTimeDto2 dto)
        {
            var availableTime = await _availableTimeOfDoctor.UpdateAvailableTimebyDoctor(id, dto);
            return Ok(availableTime);
        }
        [HttpPut("UpdateAvailableTimebyreceptionist")]
        public async Task<IActionResult> UpdateAvailableTimebyreceptionist(int id, UpdateAvailableTimeDto dto)
        {
            var availableTime = await _availableTimeOfDoctor.UpdateAvailableTimebyreceptionist(id, dto);
            return Ok(availableTime);
        }
    }
}
