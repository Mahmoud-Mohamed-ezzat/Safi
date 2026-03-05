using Microsoft.AspNetCore.Identity;
using Safi.Dto.Account;
using Safi.Models;

namespace Safi.Interfaces
{
    public interface IAccount
    {
        Task<ResponseOfLogin?> LoginAsync(LoginDto login);
        Task<IdentityResult> SignupAsSubAdminAsync(SignupFoRAdminOfWebDto model, string? imagePath);
        Task<IdentityResult> SignupAsAdminAsync(SignupFoRAdminOfWebDto model, string? imagePath);
        Task<IdentityResult> SignupAsPatientAsync(SignupAsPatientDto model, string? imagePath);
        Task<IdentityResult> SignupAsDoctorAsync(SignupOfDoctorDto model, string? imagePath);
        Task<IdentityResult> SignupAsStaffAsync(SignupOfStaffDto model, string? imagePath);
        Task<string> ForgetPasswordAsync(ForgetPasswordDto model);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto model);
        Task<bool> LogOutAsync();

        Task<List<GetDoctorsDto>> GetDoctorsAsync();
        Task<GetDoctorsDto?> GetDoctorByIdAsync(string id);
        Task<List<GetStaffsDto>> GetStaffAsync();
        Task<GetStaffsDto?> GetStaffByIdAsync(string id);
        Task<List<GetPatientsDto>> GetPatientsAsync();
        Task<GetPatientsDto?> GetPatientByIdAsync(string id);
        Task<List<GetSubAdminsDto>> GetSubAdminsAsync();
        Task<GetSubAdminsDto?> GetSubAdminByIdAsync(string id);

        Task<bool> DeleteDoctorAsync(string userId);
        Task<bool> DeletePatientAsync(string userId);
        Task<bool> DeleteStaffAsync(string userId);
        Task<bool> DeleteSubAdminAsync(string userId);
        Task<bool> ReturnFromDeleteAsync(string userId);
        Task<bool> DeactivateAccountAsync(string userId);
        Task<bool> ActivateAccountAsync(string userId);

        Task<List<GetDoctorsDto>> GetDeletedDoctorsAsync();
        Task<List<GetPatientsDto>> GetDeletedPatientsAsync();
        Task<List<GetSubAdminsDto>> GetDeactivatedUsersAsync();

        Task<User?> ExternalLoginCallbackAsync(ExternalLoginInfo info, string? imagePath);
        Task<ResponseOfLogin?> RefreshTokenAsync();

        Task<IdentityResult> UpdatePatientProfileAsync(string userId, UPdatePatientProfileDto model, string? imagePath);
        Task<IdentityResult> UpdateDoctorProfileAsync(string userId, UpdateDoctorProfileDto model, string? imagePath);
        Task<IdentityResult> UpdateStaffProfileAsync(string userId, UpdateStaffProfileDto model, string? imagePath);
        Task<IdentityResult> UpdateAdminProfileAsync(string userId, UpdateAdminProfileDto model, string? imagePath);
    }
}
