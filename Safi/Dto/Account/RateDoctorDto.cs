using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.Account
{
    public class RateDoctorDto
    {
        [Required]
        public string DoctorId { get; set; } = string.Empty;

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }
    }
}
