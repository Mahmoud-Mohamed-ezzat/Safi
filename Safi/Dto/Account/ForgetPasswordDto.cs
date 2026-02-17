using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.Account
{
    public class ForgetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string ResetLink { get; set; } = string.Empty;
    }
}
