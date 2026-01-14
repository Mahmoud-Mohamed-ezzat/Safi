using Safi.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Safi.Dto.AvailableTimeOFDoctor;

    public class CreateAvailableTimeDto
    {
        public string DoctorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateOnly Day { get; set; }
        public int Slots { get; set; }
    }

