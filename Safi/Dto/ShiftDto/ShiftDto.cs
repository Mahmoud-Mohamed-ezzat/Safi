using System;

namespace Safi.Dto.ShiftDto
{
    public class ShiftDto
    {
        public int Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
