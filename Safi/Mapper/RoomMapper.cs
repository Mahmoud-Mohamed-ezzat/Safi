using Safi.Dto.RoomDto;
using Safi.Models;

namespace Safi.Mapper
{
    public static class RoomMapper
    {
        public static RoomDto ToRoomDto(this Room room)
        {
            return new RoomDto
            {
                Id = room.Id,
                Number = room.Number,
                DepartmentId = room.DepartmentId,
                DepartmentName = room.Department?.Name
            };
        }

        public static Room ToRoom(this CreateRoomDto dto)
        {
            return new Room
            {
                Number = dto.Number,
                DepartmentId = dto.DepartmentId
            };
        }
    }
}
