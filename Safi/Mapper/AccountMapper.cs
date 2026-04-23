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
                DateOfBirth = doctor.DateOfBirth,
                DepartmentId = doctor.DepartmentId,
                Gender = doctor.Gender,
                DepartmentName = doctor.Department?.Name ?? "",
                IsDeleted = doctor.IsDeleted,
                IsActive = doctor.IsActive
            };
        }
        public static GetStaffsDto ToGetStaffsDto(this Staff staff)
        {
            return new GetStaffsDto
            {
                Id = staff.Id ?? "",
                Name = staff.Name ?? "",
                Image = staff.Image ?? "",
                Email = staff.Email ?? "",
                Phone = staff.PhoneNumber ?? "",
                University = staff.University ?? "",
                DepartmentId = staff.DepartmentId,
                Gender = staff.Gender,
                DepartmentName = staff.Department?.Name ?? "",
                DateOfBirth = staff.DateOfBirth,
                IsDeleted = staff.IsDeleted,
                IsActive = staff.IsActive
            };
        }
        public static GetPatientsDto ToGetPatientsDto(this Patient patient)
        {
            return new GetPatientsDto
            {
                Id = patient.Id,
                Name = patient.Name ?? null,
                Image = patient.Image ?? null,
                Email = patient.Email ?? null,
                Phone = patient.PhoneNumber ?? null,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                HasSugar = patient.HasSugar,
                History = patient.History ?? "",
                HasPressure = patient.HasPressure,
                Departments = patient.Departments?.Select(d => new DepartmentInfoDto
                {
                    Id = d.Id,
                    Name = d.Name ?? ""
                }).ToList(),
                IsDeleted = patient.IsDeleted,
                IsActive = patient.IsActive
            };
        }
        public static GetPatientByIdOrNameDto ToGetPatientByIdOrNameDto(this Patient patient)
        {
            return new GetPatientByIdOrNameDto
            {
                Id = patient.Id,
                CustomId = patient.Custome_Id,
                Name = patient.Name ?? null,
                Image = patient.Image ?? null,
                Email = patient.Email ?? null,
                Phone = patient.PhoneNumber ?? null,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                HasSugar = patient.HasSugar,
                History = patient.History ?? "",
                HasPressure = patient.HasPressure,
                Departments = patient.Departments?.Select(d => new DepartmentInfoDto
                {
                    Id = d.Id,
                    Name = d.Name ?? ""
                }).ToList(),
                IsDeleted = patient.IsDeleted,
                IsActive = patient.IsActive
            };
        }
        public static GetSubAdminsDto ToGetSubAdminsDto(this User user)
        {
            return new GetSubAdminsDto
            {
                Id = user.Id,
                Name = user.Name,
                Image = user.Image ?? "",
                Email = user.Email ?? "",
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Phone = user.PhoneNumber ?? "",
                IsDeleted = user.IsDeleted,
                IsActive = user.IsActive
            };
        }
    }
}
