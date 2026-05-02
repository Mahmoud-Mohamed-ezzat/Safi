namespace Safi.Dto.AssignWorksDto
{
    public class AssignWorksDto
    {
        public int Id { get; set; }
        public int? RoomId { get; set; }
        public int? RoomNumber { get; set; }
        public string? DoctorId { get; set; } // Renamed to DoctorId in DTO to keep API compatibility if needed, or I can rename to userId
        public string? DoctorName { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}
