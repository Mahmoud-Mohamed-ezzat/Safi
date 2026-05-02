using Safi.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Safi.Dto.Attendance
{
    public class CreateAttendanceDto
    {
        public DateOnly Date { get; set; }
        public string status { get; set; } //"attend,Late,Off,Abscent"
        public string? Notes { get; set; }
        public string UserrId { get; set; }
        public int ShiftId { get; set; } // from assign rooms to doctor
    }
}
