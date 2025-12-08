using Safi.Dto.Department;
using Safi.Dto.Account;
namespace Safi.Interfaces
{
    public interface IDepartment
    {
        public Task<String> RemoveDepartment(int id);
        public Task<String> UpdateDepartment(int id, DepartmentDto departmentDto);
        public Task<DepartmentInfoDto> GetDepartmentById(int id);
        public Task<List<DepartmentInfoDto>> GetAllDepartments();
        public Task<List<GetDoctorsDto>> GetDoctorsOfDepartment(int id);
        public Task<List<GetDoctorsDto>> GetDoctorsOfDepartment(string NameOfDepartment);
        public Task<List<GetPatientsDto>> GetPatientsOfDepartment(int id);
        public Task<List<GetPatientsDto>> GetPatientsOfDepartment(string NameOfDepartment);
        public Task<List<GetStaffsDto>> GetStaffOfDepartment(int id);
        public Task<List<GetStaffsDto>> GetStaffOfDepartment(string NameOfDepartment);
    }
}
