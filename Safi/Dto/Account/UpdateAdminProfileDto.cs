using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.Account
{
    public class UpdateAdminProfileDto
    {
        public string? Name { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public IFormFile? Image { get; set; }
    }
}
