namespace Safi.Dto.OutBoxDto
{
    public class BillAndEmailForAppointmentDto
    {
        public int AppointmentId { get; set; }
        public string PatientId { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string? PatientEmail { get; set; }
        public string DoctorId { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string? DoctorEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public string RoomType { get; set; } = string.Empty;
        public int RoomNumber { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
