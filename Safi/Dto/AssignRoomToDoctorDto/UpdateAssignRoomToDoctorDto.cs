namespace Safi.Dto.AssignRoomToDoctorDto
{
    public class UpdateAssignRoomToDoctorDto
    {
        public int? RoomId { get; set; }
        public string? DoctorId { get; set; }
        public TimeOnly Start_Time { get; set; }
        public TimeOnly End_Time { get; set; }
    }
}
