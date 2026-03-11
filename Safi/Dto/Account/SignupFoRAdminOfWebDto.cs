using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.Account
{
    public class SignupFoRAdminOfWebDto
    {
        public required string username { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? Gender { get; set; }
        [EmailAddress]
        public required string email { get; set; }
        public required string Password { get; set; }
        public required string Phone { get; set; }
        public IFormFile? Image { get; set; }
    }
}

