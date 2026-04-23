using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Safi.Dto.Account;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;
using Safi.Services;

namespace Safi.Repositories
{
    public class AccountRepo : IAccount
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly TokenService _tokenService;
        private readonly SafiContext _context;
        private readonly IEmailService _emailService;

        public AccountRepo(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            TokenService tokenService,
            SafiContext context,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _context = context;
            _emailService = emailService;
        }

        public async Task<ResponseOfLogin?> LoginAsync(LoginDto login)
        {
            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Email == login.Email);

            if (user == null) return null;
            if (!user.IsActive) return null; // Or throw custom exception for "Deactivated"

            var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);
            if (!result.Succeeded) return null;

            return await _tokenService.GenerateToken(user);
        }

        public async Task<IdentityResult> SignupAsSubAdminAsync(SignupFoRAdminOfWebDto model, string? imagePath)
        {
            var user = new User
            {
                Name = model.username,
                UserName = model.email,
                Gender = model.Gender,
                Email = model.email,
                PhoneNumber = model.Phone,
                DateOfBirth = model.DateOfBirth,
                Image = imagePath
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "SubAdmin");
                await _emailService.SendEmailAsync(new Dto.EmailDto.SendEmailDto
                {
                    ToEmail = model.email,
                    Subject = "SubAdmin Account Created",
                    Body = $"Dear {model.username},\n\nYour SubAdmin account has been created.\nEmail: {model.email}\nPassword: {model.Password}\n\nPlease log in and change your password as soon as possible for security reasons."
                });
            }
            return result;
        }

        public async Task<IdentityResult> SignupAsAdminAsync(SignupFoRAdminOfWebDto model, string? imagePath)
        {
            var user = new User
            {
                Name = model.username,
                UserName = model.email,
                Gender = model.Gender,
                Email = model.email,
                PhoneNumber = model.Phone,
                DateOfBirth = model.DateOfBirth,
                Image = imagePath
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            return result;
        }

        public async Task<IdentityResult> SignupAsPatientAsync(SignupAsPatientDto model, string? imagePath)
        {
            var user = new Patient
            {
                Name = model.username,
                UserName = model.email,
                Gender = model.Gender,
                Email = model.email,
                DateOfBirth = model.DateOfBirth,
                PhoneNumber = model.Phone,
                Image = imagePath,
                HasSugar = model.hassugar,
                HasPressure = model.hasPressure
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Patient");
            }
            return result;
        }

        public async Task<IdentityResult> SignupAsDoctorAsync(SignupOfDoctorDto model, string? imagePath)
        {
            var user = new Doctor
            {
                Name = model.username,
                UserName = model.email,
                Gender = model.Gender,
                Email = model.email,
                PhoneNumber = model.Phone,
                Image = imagePath,
                University = model.University,
                Degree = model.Degree,
                Rank = model.Rank,
                DepartmentId = model.DepartmentId,
                DateOfBirth = model.DateOfBirth
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Doctor");
                await _emailService.SendEmailAsync(new Dto.EmailDto.SendEmailDto
                {
                    ToEmail = model.email,
                    Subject = "Doctor Account Created",
                    Body = $"Dear {model.username},\n\nYour Doctor account has been created.\nEmail: {model.email}\nPassword: {model.Password}\n\nPlease log in and change your password as soon as possible for security reasons."
                });
            }
            return result;
        }

        public async Task<IdentityResult> SignupAsStaffAsync(SignupOfStaffDto model, string? imagePath)
        {
            var user = new Staff
            {
                Name = model.username,
                UserName = model.email,
                Email = model.email,
                PhoneNumber = model.Phone,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Image = imagePath,
                University = model.University,
                DepartmentId = model.DepartmentId
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Staff");
                await _emailService.SendEmailAsync(new Dto.EmailDto.SendEmailDto
                {
                    ToEmail = model.email,
                    Subject = "Staff Account Created",
                    Body = $"Dear {model.username},\n\nYour Staff account has been created.\nEmail: {model.email}\nPassword: {model.Password}\n\nPlease log in and change your password as soon as possible for security reasons."
                });
            }
            return result;
        }

        public async Task<string> ForgetPasswordAsync(ForgetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return "If your email is registered, you will receive a password reset link.";

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = System.Net.WebUtility.UrlEncode(token);

            await _emailService.SendEmailAsync(new Dto.EmailDto.SendEmailDto
            {
                ToEmail = model.Email,
                Subject = "Reset Password",
                Body = $"Please reset your password by clicking on the following link: {model.ResetLink}?token={encodedToken}&email={model.Email}"
            });

            return "If your email is registered, you will receive a password reset link.";
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "Invalid request" });

            return await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        }

        public async Task<bool> LogOutAsync()
        {
            await _signInManager.SignOutAsync();
            return await _tokenService.RevokeRefreshToken();
        }

        public async Task<List<GetDoctorsDto>> GetDoctorsAsync()
        {
            var doctors = await _context.Doctors.AsNoTracking()
                .Include(d => d.Department).ToListAsync();
            return doctors.Select(p => p.ToGetDoctorsDto()).ToList();
        }

        public async Task<GetDoctorsDto?> GetDoctorByIdAsync(string id)
        {
            var doctor = await _context.Doctors.AsNoTracking().Include(d => d.Department).FirstOrDefaultAsync(d => d.Id == id);
            return doctor?.ToGetDoctorsDto();
        }

        public async Task<List<GetStaffsDto>> GetStaffAsync()
        {
            var staffs = await _context.Staffs.AsNoTracking()
                .Include(d => d.Department).ToListAsync();
            return staffs.Select(p => p.ToGetStaffsDto()).ToList();
        }

        public async Task<GetStaffsDto?> GetStaffByIdAsync(string id)
        {
            var staff = await _context.Staffs.AsNoTracking()
                .Include(d => d.Department).FirstOrDefaultAsync(d => d.Id == id);
            return staff?.ToGetStaffsDto();
        }

        public async Task<List<GetPatientsDto>> GetPatientsAsync()
        {
            var patients = await _context.Patients.AsNoTracking()
                .Include(d => d.Departments).ToListAsync();
            return patients.Select(p => p.ToGetPatientsDto()).ToList();
        }

        public async Task<GetPatientsDto?> GetPatientByIdAsync(string id)
        {
            var patient = await _context.Patients.AsNoTracking()
                .Include(d => d.Departments).FirstOrDefaultAsync(d => d.Id == id);
            return patient?.ToGetPatientsDto();
        }

        public async Task<GetPatientByIdOrNameDto?> GetPatientByIdOrNameAsync(int id, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return (await _context.Patients.AsNoTracking()
                .Include(d => d.Departments)
                .FirstOrDefaultAsync(d => d.Custome_Id == id))?
                .ToGetPatientByIdOrNameDto();
            }
            return (await _context.Patients.AsNoTracking()
                .Include(d => d.Departments)
                .FirstOrDefaultAsync(d => d.Custome_Id == id || d.Name.ToLower().Contains(name.ToLower())))?
                .ToGetPatientByIdOrNameDto();
        }
        public async Task<List<GetSubAdminsDto>> GetSubAdminsAsync()
        {
            var subAdmins = await _userManager.GetUsersInRoleAsync("SubAdmin");
            return subAdmins.Select(s => s.ToGetSubAdminsDto()).ToList();
        }

        public async Task<GetSubAdminsDto?> GetSubAdminByIdAsync(string id)
        {
            var subAdmins = await _userManager.GetUsersInRoleAsync("SubAdmin");
            var subAdmin = subAdmins.FirstOrDefault(d => d.Id == id);
            return subAdmin?.ToGetSubAdminsDto();
        }

        public async Task<bool> DeleteDoctorAsync(string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var doctor = await _context.Doctors
                    .Include(d => d.Department)
                    .FirstOrDefaultAsync(d => d.Id == userId);

                if (doctor == null) return false;

                doctor.IsDeleted = true;

                // 1. Delete future reservations
                var futureReservations = _context.Reservations.Where(r => r.DoctorId == userId && r.Time > DateTime.Now);
                _context.Reservations.RemoveRange(futureReservations);

                // 2. Close active room assignments
                var activeAssignments = _context.AssignRoomToDoctors.Where(a => a.DoctorId == userId && (a.EndDate == null || a.EndDate > DateOnly.FromDateTime(DateTime.Now)));
                foreach (var assignment in activeAssignments)
                {
                    assignment.EndDate = DateOnly.FromDateTime(DateTime.Now);
                }

                // 3. Reassign active appointments
                var activeAppointments = await _context.AppointmentToRooms
                    .Where(a => a.DoctorId == userId && a.EndTime == null)
                    .ToListAsync();

                foreach (var appointment in activeAppointments)
                {
                    // Try priority 1: Another doctor in the same room right now
                    var anotherDoctorInRoom = await _context.AssignRoomToDoctors
                        .Where(a => a.RoomId == appointment.RoomId && a.DoctorId != userId && (a.EndDate == null || a.EndDate >= DateOnly.FromDateTime(DateTime.Now)))
                        .Select(a => a.DoctorId)
                        .FirstOrDefaultAsync();

                    if (anotherDoctorInRoom != null)
                    {
                        appointment.DoctorId = anotherDoctorInRoom;
                    }
                    else
                    {
                        // Try priority 2: Doctor in same shift and department
                        var currentShift = await _context.AssignRoomToDoctors
                            .Where(a => a.DoctorId == userId && a.RoomId == appointment.RoomId)
                            .Select(a => a.ShiftId)
                            .FirstOrDefaultAsync();

                        var fallbackDoctor = await _context.AssignRoomToDoctors
                            .Include(a => a.Doctor)
                            .Where(a => a.ShiftId == currentShift
                                        && a.DoctorId != userId
                                        && a.Doctor != null
                                        && doctor.DepartmentId != null
                                        && a.Doctor.DepartmentId == doctor.DepartmentId
                                        && (a.EndDate == null || a.EndDate >= DateOnly.FromDateTime(DateTime.Now)))
                            .Select(a => a.DoctorId)
                            .FirstOrDefaultAsync();

                        if (fallbackDoctor != null)
                        {
                            appointment.DoctorId = fallbackDoctor;
                        }
                        else
                        {
                            // Priority 3: Null
                            appointment.DoctorId = null;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> DeletePatientAsync(string userId)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == userId);
            if (patient == null) return false;
            patient.IsDeleted = true;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteStaffAsync(string userId)
        {
            var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.Id == userId);
            if (staff == null) return false;
            staff.IsDeleted = true;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteSubAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;
            user.IsDeleted = true;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ReturnFromDeleteAsync(string userId)
        {
            var user = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;
            user.IsDeleted = false;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeactivateAccountAsync(string userId)
        {
            var user = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;
            user.IsActive = false;
            var LockoutResult = await _userManager.SetLockoutEnabledAsync(user, true);
            if (!LockoutResult.Succeeded) return false;
            var EndLockoutResult = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
            if (!EndLockoutResult.Succeeded) return false;
            return true;
        }

        public async Task<bool> ActivateAccountAsync(string userId)
        {
            var user = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;
            user.IsActive = true;
            var LockoutResult = await _userManager.SetLockoutEndDateAsync(user, null); // Lift the lockout immediately
            if (!LockoutResult.Succeeded) return false;

            var enableLockout = await _userManager.SetLockoutEnabledAsync(user, false);
            if (!enableLockout.Succeeded) return false;
            return true;
        }

        public async Task<List<GetDoctorsDto>> GetDeletedDoctorsAsync()
        {
            var doctors = await _context.Doctors
            .IgnoreQueryFilters()
            .Where(d => d.IsDeleted)
            .ToListAsync();
            return doctors.Select(d => d.ToGetDoctorsDto()).ToList();
        }

        public async Task<List<GetPatientsDto>> GetDeletedPatientsAsync()
        {
            var patients = await _context.Patients
            .IgnoreQueryFilters()
            .Where(p => p.IsDeleted)
            .ToListAsync();
            return patients.Select(p => p.ToGetPatientsDto()).ToList();
        }

        public async Task<List<GetSubAdminsDto>> GetDeactivatedUsersAsync()
        {
            var users = await _context.Users.IgnoreQueryFilters()
            .Where(u => !u.IsActive && !u.IsDeleted).
            ToListAsync();
            return users.Select(u => u.ToGetSubAdminsDto()).ToList();
        }

        public async Task<User?> ExternalLoginCallbackAsync(ExternalLoginInfo info, string? imagePath)
        {
            var email = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return null;

            var userByEmail = await _userManager.FindByEmailAsync(email);
            if (userByEmail != null)
            {
                await _userManager.AddLoginAsync(userByEmail, info);
                if (imagePath != null) userByEmail.Image = imagePath;
                await _userManager.UpdateAsync(userByEmail);
                return userByEmail;
            }
            else
            {
                var userToUse = new User
                {
                    UserName = email,
                    Email = email,
                    Name = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
                           ?? $"{info.Principal.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value} {info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value}".Trim(),
                    Image = imagePath
                };

                var createResult = await _userManager.CreateAsync(userToUse);
                if (!createResult.Succeeded) return null;

                await _userManager.AddToRoleAsync(userToUse, "Patient");
                await _userManager.AddLoginAsync(userToUse, info);
                return userToUse;
            }
        }
        public async Task<ResponseOfLogin?> RefreshTokenAsync()
        {
            return await _tokenService.RotateBothTokensAsync();
        }

        public async Task<IdentityResult> UpdatePatientProfileAsync(UPdatePatientProfileDto model, string? imagePath)
        {
            var user = await _context.Patients.FirstOrDefaultAsync(u => u.Id == model.Id);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "Patient not found" });

            if (!string.IsNullOrEmpty(model.Name)) user.Name = model.Name;
            if (!string.IsNullOrEmpty(model.Phone)) user.PhoneNumber = model.Phone;
            if (model.HasSugar.HasValue) user.HasSugar = model.HasSugar.Value;
            if (model.HasPressure.HasValue) user.HasPressure = model.HasPressure.Value;
            if (model.DateOfBirth.HasValue) user.DateOfBirth = model.DateOfBirth.Value;
            if (!string.IsNullOrEmpty(model.History)) user.History += "\n" + model.History;
            if (imagePath != null) user.Image = imagePath;

            if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
            {
                var emailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!emailResult.Succeeded) return emailResult;
                await _userManager.SetUserNameAsync(user, model.Email);
            }

            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> UpdateDoctorProfileAsync(UpdateDoctorProfileDto model, string? imagePath)
        {
            var user = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == model.Id);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "Doctor not found" });

            if (!string.IsNullOrEmpty(model.Name)) user.Name = model.Name;
            if (!string.IsNullOrEmpty(model.Phone)) user.PhoneNumber = model.Phone;
            if (!string.IsNullOrEmpty(model.University)) user.University = model.University;
            if (!string.IsNullOrEmpty(model.Degree)) user.Degree = model.Degree;
            if (model.DepartmentId.HasValue) user.DepartmentId = model.DepartmentId.Value;
            if (model.DateOfBirth.HasValue) user.DateOfBirth = model.DateOfBirth.Value;
            if (imagePath != null) user.Image = imagePath;

            if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
            {
                var emailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!emailResult.Succeeded) return emailResult;
                await _userManager.SetUserNameAsync(user, model.Email);
            }

            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> UpdateStaffProfileAsync(UpdateStaffProfileDto model, string? imagePath)
        {
            var user = await _context.Staffs.FirstOrDefaultAsync(s => s.Id == model.Id);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "Staff not found" });

            if (!string.IsNullOrEmpty(model.Name)) user.Name = model.Name;
            if (!string.IsNullOrEmpty(model.Phone)) user.PhoneNumber = model.Phone;
            if (!string.IsNullOrEmpty(model.University)) user.University = model.University;
            if (model.DepartmentId.HasValue) user.DepartmentId = model.DepartmentId.Value;
            if (model.DateOfBirth.HasValue) user.DateOfBirth = model.DateOfBirth.Value;
            if (imagePath != null) user.Image = imagePath;

            if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
            {
                var emailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!emailResult.Succeeded) return emailResult;
                await _userManager.SetUserNameAsync(user, model.Email);
            }

            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> UpdateAdminProfileAsync(UpdateAdminProfileDto model, string? imagePath)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            if (!string.IsNullOrEmpty(model.Name)) user.Name = model.Name;
            if (!string.IsNullOrEmpty(model.Phone)) user.PhoneNumber = model.Phone;
            if (model.DateOfBirth.HasValue) user.DateOfBirth = model.DateOfBirth.Value;
            if (imagePath != null) user.Image = imagePath;

            if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
            {
                var emailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!emailResult.Succeeded) return emailResult;
                await _userManager.SetUserNameAsync(user, model.Email);
            }

            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> UpdatepressureAndSugarOfPatientAsync(UpdatePressureAndSugarOfPatientDto model)
        {
            var user = await _context.Patients.FirstOrDefaultAsync(u => u.Id == model.userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "Patient not found" });
            user.HasPressure = model.hasPressure;
            user.HasSugar    = model.hasSugar;
            return await _userManager.UpdateAsync(user);
        }
         public async Task<string?> GetUserHistoryAsync(string userId)
        {
            var patient = await _context.Patients.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (patient == null) return null;
            return patient.History;
        }
    }
}
