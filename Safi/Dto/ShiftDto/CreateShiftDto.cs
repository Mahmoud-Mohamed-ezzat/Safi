using System;
using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.ShiftDto
{
    public class CreateShiftDto
    {
        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }
    }
}
