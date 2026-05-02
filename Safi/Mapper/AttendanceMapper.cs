using Safi.Models;

namespace Safi.Mapper
{
    public static class AttendanceMapper
    {
        public static Attendance ToCreateAttendance(this Dto.Attendance.CreateAttendanceDto dto)
        {
            return new Attendance
            {
                Date = dto.Date,
                status = dto.status,
                Notes = dto.Notes,
                UserrId = dto.UserrId,
                ShiftId = dto.ShiftId
            };

        }
        public static Dto.Attendance.GetAttendanceDto ToGetAttendanceDto(this Attendance model)
        {
            return new Dto.Attendance.GetAttendanceDto
            {
                Id = model.Id,
                Date = model.Date,
                status = model.status,
                Notes = model.Notes,
                UserrId = model.UserrId,
                ShiftId = model.ShiftId,
                username = model.Doctor.Name
            };

        }


    }
}
