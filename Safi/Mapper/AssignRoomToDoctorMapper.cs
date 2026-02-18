using Safi.Dto.AssignRoomToDoctorDto;
using Safi.Models;

namespace Safi.Mapper
{
    public static class AssignRoomToDoctorMapper
    {
        public static AssignRoomToDoctorDto ToAssignRoomToDoctorDto(this AssignRoomToDoctor assign)
        {
            return new AssignRoomToDoctorDto
            {
                Id = assign.Id,
                RoomId = assign.RoomId,
                RoomNumber = assign.Room?.Number,
                DoctorId = assign.DoctorId,
                DoctorName = assign.Doctor?.Name,
                StartTime = assign.Shift != null ? assign.Shift.StartTime : default,
                EndTime = assign.Shift != null ? assign.Shift.EndTime : default,
                StartDate = assign.StartDate,
                EndDate = assign.EndDate
            };
        }

        public static AssignRoomToDoctor ToCreateAssignRoomToDoctorDto(this CreateAssignRoomToDoctorDto dto)
        {
            return new AssignRoomToDoctor
            {
                RoomId = dto.RoomId,
                DoctorId = dto.DoctorId,
                ShiftId = dto.ShiftId,
                StartDate = dto.StartDate != null ? dto.StartDate.Value : DateOnly.FromDateTime(DateTime.Now), // Assuming the assignment starts today
                EndDate = dto.EndDate != null ? dto.EndDate.Value : null,
            };
        }
    }
}
