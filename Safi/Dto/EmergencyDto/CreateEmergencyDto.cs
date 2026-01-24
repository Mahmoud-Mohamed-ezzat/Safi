using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.EmergencyDto
{
    public class CreateEmergencyDto
    {
        [Required]
        public int Number { get; set; }
        [Required]
        public int DepartmentId { get; set; }
    }
}
