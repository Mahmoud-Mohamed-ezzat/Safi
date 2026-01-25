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
                AppointmentToRoomId = assign.AppointmentToRoomId,
                Time = assign.Time
            };
        }

        public static AssignRoomToDoctor ToAssignRoomToDoctor(this CreateAssignRoomToDoctorDto dto)
        {
            return new AssignRoomToDoctor
            {
                RoomId = dto.RoomId,
                DoctorId = dto.DoctorId,
                AppointmentToRoomId = dto?.AppointmentToRoomId,
                Time = DateTime.Now // Or should this come from DTO? Defaulting to Now as per Model default
            };
        }
    }
}
