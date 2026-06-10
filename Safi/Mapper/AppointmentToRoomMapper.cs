using Safi.Dto.AppointmentToRoom;
using Safi.Helpers;
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
                StartTime = dto.StartTime ?? EgyptTime.Now,
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
                StartTime = entity.StartTime.HasValue ? EgyptTime.FromUtc(entity.StartTime.Value) : null,
                EndTime = entity.EndTime.HasValue ? EgyptTime.FromUtc(entity.EndTime.Value) : null,
                IsDeleted = entity.Patient?.IsDeleted ?? false,
                IsActive = entity.Patient?.IsActive ?? true,
                BillId = entity.BillId
            };
        }

        public static AvailableRoomInfoDto ToAvailableRoomInfoDto(this Room room, List<AssignWorks> assignments)
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
                    .Where(a => a.user is Doctor)
                    .Select(a => (Doctor)a.user!)
                    .Select(d => new AssignedDoctorDto
                    {
                        DoctorId = d.Id,
                        DoctorName = d.UserName ?? string.Empty,
                        Degree = d.Degree,
                        University = d.University,
                        Rank = d.Rank,

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
