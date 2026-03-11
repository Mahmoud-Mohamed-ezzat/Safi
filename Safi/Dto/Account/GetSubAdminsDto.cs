namespace Safi.Dto.Account
{
    public class GetSubAdminsDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Email { get; set; }
        public string? Gender { get; set; }
        public string Phone { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}
