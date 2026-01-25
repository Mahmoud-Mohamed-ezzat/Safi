namespace Safi.Dto.AssignRoomToDoctorDto
{
    public class UpdateAssignRoomToDoctorDto
    {
        public int? RoomId { get; set; }
        public string? DoctorId { get; set; }
        public int? AppointmentToRoomId { get; set; }
    }
}
