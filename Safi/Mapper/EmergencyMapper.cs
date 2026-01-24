using Safi.Dto.EmergencyDto;
using Safi.Models;

namespace Safi.Mapper
{
    public static class EmergencyMapper
    {
        public static EmergencyDto ToEmergencyDto(this Emergency emergency)
        {
            return new EmergencyDto
            {
                Id = emergency.Id,
                Number = emergency.Number,
                DepartmentId = emergency.DepartmentId,
                DepartmentName = emergency.Department?.Name
            };
        }

        public static Emergency ToEmergency(this CreateEmergencyDto dto)
        {
            return new Emergency
            {
                Number = dto.Number,
                DepartmentId = dto.DepartmentId
            };
        }
    }
}
