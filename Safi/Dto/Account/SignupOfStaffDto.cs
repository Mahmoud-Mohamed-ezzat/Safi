using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.Account
{
    public class SignupOfStaffDto
    {
        public string username { get; set; }
        [EmailAddress]
        public string email { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public IFormFile? Image { get; set; }
        public string University { get; set; } // University graduated from
        public int DepartmentId { get; set; }
    }
}
