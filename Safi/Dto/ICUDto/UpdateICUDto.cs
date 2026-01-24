using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.ICUDto
{
    public class UpdateICUDto
    {
        [Required]
        public int Number { get; set; }
        [Required]
        public int DepartmentId { get; set; }
    }
}
