using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.Account
{
    public class SignupOfDoctorDto
    {
        public string username { get; set; }
        [EmailAddress]
        public string email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public IFormFile? Image { get; set; }
        public string University { get; set; } // University graduated from
        public string Degree { get; set; } // Medical degree
        public float Rank { get; set; } // Ranking or rating
        public int DepartmentId { get; set; }
    }
}
