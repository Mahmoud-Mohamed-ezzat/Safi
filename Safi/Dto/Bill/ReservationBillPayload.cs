namespace Safi.Dto.Bill
{
    public class ReservationBillPayload
    {
        public int ReservationId { get; set; }
        public string PatientId { get; set; } = string.Empty;
        public string? DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string? DoctorEmail { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string? PatientEmail { get; set; }
        public DateTime Time { get; set; }
    }
}
