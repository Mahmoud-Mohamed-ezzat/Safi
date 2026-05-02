namespace Safi.Dto.Attendance
{
    public class UpdateAttendanceDto
    {
        public string status { get; set; } //"attend,Late,Off,Abscent"
        public string? Notes { get; set; }
    }
}
