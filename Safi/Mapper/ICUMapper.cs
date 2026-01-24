using Safi.Dto.ICUDto;
using Safi.Models;

namespace Safi.Mapper
{
    public static class ICUMapper
    {
        public static ICUDto ToICUDto(this ICU icu)
        {
            return new ICUDto
            {
                Id = icu.Id,
                Number = icu.Number,
                DepartmentId = icu.DepartmentId,
                DepartmentName = icu.Department?.Name
            };
        }

        public static ICU ToICU(this CreateICUDto dto)
        {
            return new ICU
            {
                Number = dto.Number,
                DepartmentId = dto.DepartmentId
            };
        }
    }
}
