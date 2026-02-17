using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.AppointmentToRoom
{
    public class CreateAppointmentToRoomDto
    {
        [Required]
        public required string CreatedBy { get; set; }

        [Required]
        public required string PatientId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        public required string PrimaryDoctorId { get; set; }

        public DateTime? StartTime { get; set; }
    }
}
