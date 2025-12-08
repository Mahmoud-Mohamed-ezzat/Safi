using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.Account
{
    public class SignupAsPatientDto
    {
        public string username { get; set; }
        [EmailAddress]

        public string email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public bool hassugar { get; set; }
        public bool hasPressure { get; set; }
        public IFormFile? Image { get; set; }
    }
}

