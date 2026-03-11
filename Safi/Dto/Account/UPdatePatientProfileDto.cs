using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.Account
{
    public class UPdatePatientProfileDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool? HasSugar { get; set; }
        public bool? HasPressure { get; set; }
        public string? History { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public IFormFile? Image { get; set; }
    }
}
