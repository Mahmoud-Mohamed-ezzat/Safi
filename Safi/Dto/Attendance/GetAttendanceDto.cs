namespace Safi.Dto.Attendance
{
    public class GetAttendanceDto
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public string status { get; set; } //"attend,Late,Off,Abscent"
        public string? Notes { get; set; }
        public string UserrId { get; set; }
        public string username { get; set; }
        public int ShiftId { get; set; } // from assign rooms to doctor
    }
}
