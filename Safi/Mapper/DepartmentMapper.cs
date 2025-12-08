using Safi.Dto.Department;
using Safi.Models;

namespace Safi.Mapper
{
    public static class DepartmentMapper
    {
        public static DepartmentDto ToDepartmentDto(this Department department)
        {
            return new DepartmentDto
            {
                Name = department.Name,
            };
        }

        public static DepartmentInfoDto ToDepartmentInfoDto(this Department department)
        {
            return new DepartmentInfoDto
            {
                Id = department.Id,
                Name = department.Name
            };
        }
    }

}
