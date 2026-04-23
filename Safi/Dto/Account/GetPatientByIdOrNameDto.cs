using Safi.Dto.Department;

namespace Safi.Dto.Account
{
    public class GetPatientByIdOrNameDto
    {
        public string Id { get; set; }
        public int CustomId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Email { get; set; }
        public string? Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string? History { get; set; }
        public bool? HasSugar { get; set; } // Diabetes indicator
        public bool? HasPressure { get; set; } // Hypertension indicator
        public List<DepartmentInfoDto>? Departments { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}
