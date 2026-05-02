namespace Safi.Dto.AssignWorksDto
{
    public class UpdateAssignWorksDto
    {
        public int? RoomId { get; set; }
        public string? DoctorId { get; set; }
        public int? ShiftId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
