namespace Safi.Dto.EmergencyDto
{
    public class EmergencyDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
    }
}
