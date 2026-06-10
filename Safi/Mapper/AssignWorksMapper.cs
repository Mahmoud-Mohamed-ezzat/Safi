using Safi.Dto.AssignWorksDto;
using Safi.Helpers;
using Safi.Models;

namespace Safi.Mapper
{
    public static class AssignWorksMapper
    {
        public static AssignWorksDto ToAssignRoomToDoctorDto(this AssignWorks assign)
        {
            return new AssignWorksDto
            {
                Id = assign.Id,
                RoomId = assign.RoomId,
                RoomNumber = assign.Room?.Number,
                DoctorId = assign.userId,
                DoctorName = assign.user?.Name,
                StartTime = assign.Shift != null ? assign.Shift.StartTime : default,
                EndTime = assign.Shift != null ? assign.Shift.EndTime : default,
                StartDate = assign.StartDate,
                EndDate = assign.EndDate,
                IsDeleted = assign.user?.IsDeleted ?? false,
                IsActive = assign.user?.IsActive ?? true
            };
        }

        public static AssignWorks ToCreateAssignRoomToDoctorDto(this CreateAssignWorksDto dto)
        {
            return new AssignWorks
            {
                RoomId = dto.RoomId,
                userId = dto.DoctorId,
                ShiftId = dto.ShiftId,
                StartDate = dto.StartDate != null ? dto.StartDate.Value : EgyptTime.Today,
                EndDate = dto.EndDate != null ? dto.EndDate.Value : null,
            };
        }
    }
}
