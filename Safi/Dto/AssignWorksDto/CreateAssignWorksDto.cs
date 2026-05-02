using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.AssignWorksDto
{
    public class CreateAssignWorksDto
    {
        [Required]
        public int RoomId { get; set; }
        [Required]
        public string DoctorId { get; set; } // Renamed in model to userId, but keeping DTO property name for now
        [Required]
        public int ShiftId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
