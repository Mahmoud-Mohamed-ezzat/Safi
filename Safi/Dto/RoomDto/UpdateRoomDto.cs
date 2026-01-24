using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.RoomDto
{
    public class UpdateRoomDto
    {
        [Required]
        public int Number { get; set; }
        [Required]
        public int DepartmentId { get; set; }
    }
}
