namespace Safi.Dto.AssignRoomToDoctorDto
{
    public class AssignRoomToDoctorDto
    {
        public int Id { get; set; }
        public int? RoomId { get; set; }
        public int? RoomNumber { get; set; }
        public string? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public int? AppointmentToRoomId { get; set; }
        public DateTime Time { get; set; }
    }
}
