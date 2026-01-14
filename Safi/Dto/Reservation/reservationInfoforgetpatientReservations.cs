namespace Safi.Dto.Reservation
{
    public class reservationInfoforgetpatientReservations
    {
        public string? PatientId { get; set; }
        public string? PatientName { get; set; }
        public string DoctorId { get; set; }    

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Status { get; set; }
    }
}
