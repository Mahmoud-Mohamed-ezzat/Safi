namespace Safi.Dto.AssignRoomToDoctorDto
{
    public class AssignRoomToDoctorDto
    {
        public int Id { get; set; }
        public int? RoomId { get; set; }
        public int? RoomNumber { get; set; }
        public string? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
