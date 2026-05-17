using System;

namespace Safi.Dto.OutBoxDto
{
    public class CreateAppointmentEmailDto
    {
        public int AppointmentId { get; set; }
        public string? PatientId { get; set; }
        public string? PatientName { get; set; }
        public string? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public string? DoctorEmail { get; set; }
        public string? RoomType { get; set; }
        public string? RoomNumber { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime StartTime { get; set; }
    }
}
