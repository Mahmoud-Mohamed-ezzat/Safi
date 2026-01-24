using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.EmergencyDto
{
    public class UpdateEmergencyDto
    {
        [Required]
        public int Number { get; set; }
        [Required]
        public int DepartmentId { get; set; }
    }
}
