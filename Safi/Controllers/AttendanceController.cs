using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Safi.Dto.Attendance;
using Safi.Interfaces;

namespace Safi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendance _attendanceRepo;

        public AttendanceController(IAttendance attendanceRepo)
        {
            _attendanceRepo = attendanceRepo;
        }

        // [Authorize(Roles = "Admin,subadmin,Staff")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var attendances = await _attendanceRepo.GetAllAsync();
            return Ok(attendances);
        }

        // [Authorize(Roles = "Admin,subadmin,Staff")]
        [HttpGet("today")]
        public async Task<IActionResult> GetAllToday()
        {
            var attendances = await _attendanceRepo.GetAllTodayAsync();
            return Ok(attendances);
        }

        [Authorize(Roles = "Admin,subadmin,Staff")]
        [HttpGet("date/{date}")]
        public async Task<IActionResult> GetAllByDate(DateOnly date)
        {
            var attendances = await _attendanceRepo.GetAllByDateAsync(date);
            return Ok(attendances);
        }

        // [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var attendance = await _attendanceRepo.GetByIdAsync(id);
            if (attendance == null)
            {
                return NotFound("Attendance record not found.");
            }
            return Ok(attendance);
        }

        // [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var attendance = await _attendanceRepo.GetByIdOfDoctorAsync(userId);
            if (attendance == null)
            {
                return NotFound("No attendance found for this user.");
            }
            return Ok(attendance);
        }

        // [Authorize]
        [HttpGet("user/{userId}/date/{date}")]
        public async Task<IActionResult> GetByUserIdAndDate(string userId, DateOnly date)
        {
            var attendances = await _attendanceRepo.GetByIdAndDateOfDoctorAsync(userId, date);
            return Ok(attendances);
        }

        // [Authorize(Roles = "Admin,subadmin,Staff")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAttendanceDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var attendance = await _attendanceRepo.CreateAttendanceAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = attendance.Id }, attendance);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // [Authorize(Roles = "Admin,subadmin,Staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAttendanceDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var attendance = await _attendanceRepo.UpdateAttendanceAsync(id, dto);
                return Ok(attendance);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // [Authorize(Roles = "Admin,subadmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _attendanceRepo.DeleteAttendanceAsync(id);
            if (!success)
            {
                return NotFound("Attendance record not found.");
            }
            return NoContent();
        }


        [Authorize(Roles = "Admin,subadmin,Staff")]
        [HttpGet("date/{date}/shift/{shiftid}")]
        public async Task<IActionResult> GetAllByDateAndShift(DateOnly date, int shiftid)
        {
            var attendances = await _attendanceRepo.GetAllByDateAndShiftAsync(date, shiftid);
            return Ok(attendances);
        }
    }
}
