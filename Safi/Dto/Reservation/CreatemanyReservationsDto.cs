namespace Safi.Dto.Reservation
{
    public class CreatemanyReservationsDto
    {
        public string? PatientId { get; set; }

        public string DoctorId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int slots { get; set; }

        public string? Status { get; set; }
    }
}
