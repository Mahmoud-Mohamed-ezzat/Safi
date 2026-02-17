using Safi.Dto.ShiftDto;
using Safi.Models;

namespace Safi.Mapper
{
    public static class ShiftMapper
    {
        public static ShiftDto ToShiftDto(this Shift shift)
        {
            return new ShiftDto
            {
                Id = shift.Id,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime
            };
        }

        public static Shift ToShift(this CreateShiftDto dto)
        {
            return new Shift
            {
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };
        }
        public static GetDoctorsshiftDto toGetDoctorsshiftDto(this Doctor doctor, int room_id)
        {
            return new GetDoctorsshiftDto
            {
                Id = doctor.Id,
                Name = doctor.Name,
                Image = doctor.Image,
                Email = doctor.Email,
                Phone = doctor.PhoneNumber,
                University = doctor.University,  // University graduated from
                Degree = doctor.Degree, // Medical degree
                Rank = doctor.Rank,// Ranking or rating
                DepartmentId = doctor.DepartmentId,
                DepartmentName = doctor.Department.Name,
                room_id = room_id,
                room_number = doctor.AssignRoomToDoctors?.FirstOrDefault(a => a.RoomId == room_id)?.Room?.Number??-1, // i set -1 to test it 
            };
        }
    }
    }
    
