namespace Safi.Dto.AppointmentToRoom
{
    public class AppointmentToRoomDto
    {
        public int Id { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public string? PatientId { get; set; }
        public string? PatientName { get; set; }
        public int? RoomId { get; set; }
        public string? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public int? RoomNumber { get; set; }
        public string? RoomType { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
