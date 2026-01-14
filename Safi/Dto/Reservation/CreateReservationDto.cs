using Safi.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Safi.Dto.Reservation
{
    public class CreateReservationDto
    {

        public string? PatientId { get; set; }

        public string DoctorId { get; set; }

        public DateTime Time { get; set; }

        public string? Status { get; set; }  // "reserved", "completed", "un-reserved"
    }
}
