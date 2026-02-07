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
        public TimeOnly Start_Time { get; set; }
        public TimeOnly End_Time { get; set; }
        public DateOnly start_Date { get; set; }
        public DateOnly? End_Date { get; set; }
    }
}
