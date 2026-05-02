using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Safi.Dto.Account
{
    public class SignupOfNurseDto
    {
        public string username { get; set; }
        [EmailAddress]
        public string email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public IFormFile? Image { get; set; }
        public string University { get; set; } // University attended
        public int DepartmentId { get; set; }
    }
}
