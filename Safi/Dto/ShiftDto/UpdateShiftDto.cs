using System;

namespace Safi.Dto.ShiftDto
{
    public class UpdateShiftDto
    {
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
    }
}
