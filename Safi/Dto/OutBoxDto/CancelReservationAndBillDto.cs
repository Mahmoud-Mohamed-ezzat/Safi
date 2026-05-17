namespace Safi.Dto.OutBoxDto
{
    public class CancelReservationAndBillDto
    {
        public int ReservationId { get; set; }
        public string PatientId { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string? PatientEmail { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string DoctorId { get; set; } = string.Empty;
        public DateTime ReservationTime { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }
}
