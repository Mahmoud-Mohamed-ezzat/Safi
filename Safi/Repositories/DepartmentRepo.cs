using Microsoft.EntityFrameworkCore;
using Safi.Dto.Department;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;
using Safi.Dto.Account;
namespace Safi.Repositories
{
    public class DepartmentRepo : IDepartment
    {
        private readonly SafiContext _context;
        public DepartmentRepo(SafiContext context)
        {
            _context = context;
        }
        public async Task<List<DepartmentInfoDto>> GetAllDepartments()
        {
            var departments = await _context.Departments.AsNoTracking().ToListAsync();
            var departmentsDto = departments.Select(d => d.ToDepartmentInfoDto()).ToList();
            return departmentsDto;
        }

        public async Task<DepartmentInfoDto> GetDepartmentById(int id)
        {
            var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
            var departmentDto = department!.ToDepartmentInfoDto();
            return departmentDto;
        }

        public async Task<string> RemoveDepartment(int id)
        {
            var Department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
            if (Department != null)
            {
                _context.Departments.Remove(Department);
                await _context.SaveChangesAsync();
                return Department.Name;
            }
            return null;
        }
        public async Task<string> UpdateDepartment(int id, DepartmentDto departmentDto)
        {
            var Department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
            if (Department != null)
            {
                Department.Name = departmentDto.Name;
                await _context.SaveChangesAsync();
                return Department.Name;
            }
            return null;
        }
        public async Task<List<GetDoctorsDto>> GetDoctorsOfDepartment(int id)
        {
            var doctors = await _context.Doctors.Include(d => d.Department).Where(d => d.DepartmentId == id).ToListAsync();
            var doctorsDto = doctors.Select(d => d.ToGetDoctorsDto()).OrderByDescending(d => d.Rank).ToList();
            return doctorsDto;
        }
        public async Task<List<GetDoctorsDto>> GetDoctorsOfDepartment(string name)
        {
            var doctors = await _context.Doctors.Include(d => d.Department).Where(d => d.Department.Name.ToLower().Contains(name.ToLower())).ToListAsync();
            var doctorsDto = doctors.Select(d => d.ToGetDoctorsDto()).OrderByDescending(d => d.Rank).ToList();
            return doctorsDto;
        }
        public async Task<List<GetPatientsDto>> GetPatientsOfDepartment(int id)
        {
            var patients = await _context.Patients.Include(d => d.Departments).Where(d => d.Departments!.Any(d => d.Id == id)).ToListAsync();
            var patientsDto = patients.Select(d => d.ToGetPatientsDto()).ToList();
            return patientsDto;
        }
        public async Task<List<GetPatientsDto>> GetPatientsOfDepartment(string name)
        {
            var patients = await _context.Patients.Include(d => d.Departments).Where(d => d.Departments!.Any(d => d.Name.ToLower().Contains(name.ToLower()))).ToListAsync();
            var patientsDto = patients.Select(d => d.ToGetPatientsDto()).ToList();
            return patientsDto;
        }
        public async Task<List<GetStaffsDto>> GetStaffOfDepartment(int id)
        {
            var staff = await _context.Staffs.Include(d => d.Department).Where(d => d.DepartmentId == id).ToListAsync();
            var staffDto = staff.Select(s => s.ToGetStaffsDto()).ToList();
            return staffDto;
        }
        public async Task<List<GetStaffsDto>> GetStaffOfDepartment(string name)
        {
            var staff = await _context.Staffs.Include(d => d.Department).Where(d => d.Department.Name.ToLower().Contains(name.ToLower()) ).ToListAsync();
            var staffDto = staff.Select(s => s.ToGetStaffsDto()).ToList();
            return staffDto;
        }
    }
}
