using Safi.Dto.AppointmentToRoom;
using Safi.Models;

namespace Safi.Mapper
{
    public static class AppointmentToRoomMapper
    {
        public static AppointmentToRoom ToAppointmentToRoom(this CreateAppointmentToRoomDto dto)
        {
            return new AppointmentToRoom
            {
                CreatedBy = dto.CreatedBy,
                PatientId = dto.PatientId,
                RoomId = dto.RoomId,
                DoctorId = dto.PrimaryDoctorId,
                StartTime = dto.StartTime ?? DateTime.UtcNow,
            };
        }

        public static AppointmentToRoomDto ToAppointmentToRoomDto(this AppointmentToRoom entity)
        {
            return new AppointmentToRoomDto
            {
                Id = entity.Id,
                CreatedBy = entity.CreatedBy,
                CreatedByName = entity.CreatedByUser?.UserName,
                PatientId = entity.PatientId,
                PatientName = entity.Patient?.Name,
                DoctorId = entity.DoctorId,
                DoctorName = entity.Doctor?.Name,
                RoomId = entity.RoomId,
                RoomNumber = entity.Room?.Number,
                RoomType = entity.Room?.GetType().Name,
                DepartmentId = entity.Room?.DepartmentId,
                DepartmentName = entity.Room?.Department?.Name,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                IsDeleted = entity.Patient?.IsDeleted ?? false,
                IsActive = entity.Patient?.IsActive ?? true
            };
        }

        public static AvailableRoomInfoDto ToAvailableRoomInfoDto(this Room room, List<AssignRoomToDoctor> assignments)
        {
            return new AvailableRoomInfoDto
            {
                Id = room.Id,
                Number = room.Number,
                Status = room.Status,
                DepartmentId = room.DepartmentId,
                DepartmentName = room.Department?.Name ?? string.Empty,
                RoomType = room.GetType().Name,
                AssignedDoctors = assignments
                    .Where(a => a.Doctor != null)
                    .Select(a => new AssignedDoctorDto
                    {
                        DoctorId = a.DoctorId ?? string.Empty,
                        DoctorName = a.Doctor!.UserName ?? string.Empty,
                        Degree = a.Doctor.Degree,
                        University = a.Doctor.University,
                        Rank = a.Doctor.Rank,

                    }).ToList()
            };
        }

        public static AssignedDoctorDto ToAssignedDoctorDto(this Doctor doctor)
        {
            return new AssignedDoctorDto
            {
                DoctorId = doctor.Id,
                DoctorName = doctor.UserName ?? string.Empty,
                Degree = doctor.Degree,
                University = doctor.University,
                Rank = doctor.Rank
            };
        }
    }
}
