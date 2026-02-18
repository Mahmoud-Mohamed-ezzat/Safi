using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.AssignRoomToDoctorDto
{
    public class CreateAssignRoomToDoctorDto
    {
        [Required]
        public int RoomId { get; set; }
        [Required]
        public string DoctorId { get; set; } = string.Empty;
        [Required]
        public int ShiftId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
