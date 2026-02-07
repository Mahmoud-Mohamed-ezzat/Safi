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
                Start_Time = assign.Start_Time,
                End_Time = assign.End_Time,
                start_Date = assign.StartDate,
                End_Date = assign.EndDate
            };
        }

        public static AssignRoomToDoctor ToCreateAssignRoomToDoctorDto(this CreateAssignRoomToDoctorDto dto)
        {
            return new AssignRoomToDoctor
            {
                RoomId = dto.RoomId,
                DoctorId = dto.DoctorId,
                Start_Time= dto.Start_Time,
                End_Time = dto.End_Time,
                StartDate= DateOnly.FromDateTime(DateTime.Now) // Assuming the assignment starts today
            };
        }
    }
}
