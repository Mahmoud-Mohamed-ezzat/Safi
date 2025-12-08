namespace Safi.Dto.Account
{
    public class GetStaffsDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string University { get; set; } // University graduated from
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
    }
}
