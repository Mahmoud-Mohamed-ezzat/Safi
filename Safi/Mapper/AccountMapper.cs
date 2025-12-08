using Safi.Dto.Account;
using Safi.Dto.Department;
using Safi.Models;

namespace Safi.Mapper
{
    public static class AccountMapper
    {
        public static GetDoctorsDto ToGetDoctorsDto(this Doctor doctor)
        {
            return new GetDoctorsDto
            {
                Id = doctor.Id,
                Name = doctor.Name,
                Image = doctor.Image,
                Email = doctor.Email,
                Phone = doctor.PhoneNumber,
                University = doctor.University,
                Degree = doctor.Degree,
                Rank = doctor.Rank,
                DepartmentId = doctor.DepartmentId,
                DepartmentName = doctor.Department.Name
            };
        }
        public static GetStaffsDto ToGetStaffsDto(this Staff staff)
        {
            return new GetStaffsDto
            {
                Id = staff.Id,
                Name = staff.Name,
                Image = staff.Image,
                Email = staff.Email,
                Phone = staff.PhoneNumber,
                University = staff.University,
                DepartmentId = staff.DepartmentId,
                DepartmentName = staff.Department.Name
            };
        }
        public static GetPatientsDto ToGetPatientsDto(this Patient patient)
        {
            return new GetPatientsDto
            {
                Id = patient.Id,
                Name = patient.Name,
                Image = patient.Image,
                Email = patient.Email,
                Phone = patient.PhoneNumber,
                HasSugar = patient.HasSugar,
                History = patient.History,
                HasPressure = patient.HasPressure,
                Departments = patient.Departments?.Select(d => new DepartmentInfoDto
                {
                    Id = d.Id,
                    Name = d.Name
                }).ToList()
            };
        }

        public static GetSubAdminsDto ToGetSubAdminsDto(this User user)
        {
            return new GetSubAdminsDto
            {
                Id = user.Id,
                Name = user.Name,
                Image = user.Image,
                Email = user.Email,
                Phone = user.PhoneNumber,
            };
        }
    }
}
