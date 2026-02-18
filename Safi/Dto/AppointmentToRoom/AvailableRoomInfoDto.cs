namespace Safi.Dto.AppointmentToRoom
{
    public class AvailableRoomInfoDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Status { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty; // "Room", "ICU", "Emergency"
        public List<AssignedDoctorDto> AssignedDoctors { get; set; } = new List<AssignedDoctorDto>();
    }
}
