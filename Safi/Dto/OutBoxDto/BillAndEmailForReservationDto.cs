namespace Safi.Dto.OutBoxDto
{
public class BillAndEmailForReservationDto
    {
        public int ReservationId { get; set; }
        public string PatientId { get; set; } = string.Empty;
        public string DoctorId { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string DoctorEmail { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;
    }
}
