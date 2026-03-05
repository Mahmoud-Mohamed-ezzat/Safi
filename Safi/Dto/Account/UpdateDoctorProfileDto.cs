using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.Account
{
    public class UpdateDoctorProfileDto
    {
        public string? Name { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? University { get; set; }
        public string? Degree { get; set; }
        public float? Rank { get; set; }
        public int? DepartmentId { get; set; }
        public IFormFile? Image { get; set; }
    }
}
