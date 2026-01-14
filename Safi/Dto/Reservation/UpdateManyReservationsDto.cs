using System;

namespace Safi.Dto.Reservation
{
    public class UpdateManyReservationsDto
    {
        public required string DoctorId { get; set; }
        public string? PatientId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int slots { get; set; }
        public string? Status { get; set; }
    }
}
