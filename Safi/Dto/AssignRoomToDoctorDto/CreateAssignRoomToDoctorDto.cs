using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.AssignRoomToDoctorDto
{
    public class CreateAssignRoomToDoctorDto
    {
        [Required]
        public int RoomId { get; set; }
        [Required]
        public string DoctorId { get; set; } = string.Empty;
        public int? AppointmentToRoomId { get; set; }
    }
}
