using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Safi.Dto.Account;
using Safi.Dto.EmailDto;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;
using Safi.Services;


using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Safi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        readonly UserManager<User> _userManager;
        readonly SignInManager<User> _signInManager;
        readonly TokenService _tokenServices;
        readonly ImageService _imageService;
        readonly IAccount _accountRepo;
        readonly SafiContext _context;
        readonly IWebHostEnvironment _env;

        public AccountsController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            TokenService tokenServices,
            ImageService imageService,
            IAccount accountRepo,
            SafiContext context,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _tokenServices = tokenServices;
            _imageService = imageService;
            _accountRepo = accountRepo;
            _env = env;
        }

        [AllowAnonymous]
        [HttpGet("external-login/google")]
        public IActionResult GoogleLogin(string returnUrl = "/")
        {
            var redirectUrl = $"{Request.Scheme}://{Request.Host}/api/accounts/external-login/google/callback";
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            properties.Items["returnUrl"] = returnUrl;
            properties.SetParameter("prompt", "select_account");
            return Challenge(properties, "Google");
        }
        [AllowAnonymous]
        [HttpGet("external-login/google/callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return BadRequest("Error loading external login information.");

            string? imagePath = null;
            var pictureUrl = info.Principal.FindFirst("picture")?.Value;
            if (!string.IsNullOrEmpty(pictureUrl))
            {
                imagePath = await DownloadAndSaveGoogleImage(pictureUrl);
            }

            var userToUse = await _accountRepo.ExternalLoginCallbackAsync(info, imagePath);
            if (userToUse == null) return BadRequest("Could not sign in with Google.");

            await _signInManager.SignInAsync(userToUse, isPersistent: false);
            var jwt = await _tokenServices.GenerateToken(userToUse);

            if (!string.IsNullOrEmpty(jwt.RefreshToken))
            {
                _tokenServices.SetRefreshTokenInCookies(jwt.RefreshToken, jwt.RefreshTokenExpiration);
            }

            var returnUrl = info.AuthenticationProperties?.Items["returnUrl"] ?? "/";

            return Redirect($"{returnUrl}?token={jwt.Token}");
        }

        private async Task<string?> DownloadAndSaveGoogleImage(string imageUrl)
        {
            try
            {
                using var client = new HttpClient();
                var imageBytes = await client.GetByteArrayAsync(imageUrl);
                var fileName = $"google_{Guid.NewGuid()}.jpg";
                var imagesFolder = Path.Combine(_env.WebRootPath, "images");
                Directory.CreateDirectory(imagesFolder);
                var filePath = Path.Combine(imagesFolder, fileName);
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
                // Return a frontend - accessible relative URL
                return $"/images/{fileName}";
            }
            catch { return null; }
        }
        [AllowAnonymous]
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _accountRepo.ForgetPasswordAsync(model);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _accountRepo.ResetPasswordAsync(model);
            if (result.Succeeded) return Ok("Password reset successful");
            return BadRequest(result.Errors.Select(e => e.Description));
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("SignUpSubAdmin")]
        public async Task<IActionResult> signupAsSubAdmin([FromForm] SignupFoRAdminOfWebDto model)
        {
            if (!ModelState.IsValid) return BadRequest();
            string? imagePath = model.Image != null ? await _imageService.SaveImageAsync(model.Image) : null;
            var result = await _accountRepo.SignupAsSubAdminAsync(model, imagePath);
            if (result.Succeeded) return Ok("Registered successful");
            return StatusCode(500, new { errors = result.Errors.Select(e => e.Description) });
        }

        [AllowAnonymous]
        [HttpPost("SignUpAdmin")]
        public async Task<IActionResult> signupAsAdmin([FromForm] SignupFoRAdminOfWebDto model)
        {
            if (!ModelState.IsValid) return BadRequest();
            string? imagePath = model.Image != null ? await _imageService.SaveImageAsync(model.Image) : null;
            var result = await _accountRepo.SignupAsAdminAsync(model, imagePath);
            if (result.Succeeded) return Ok("Registered successful");
            return StatusCode(500, new { errors = result.Errors.Select(e => e.Description) });
        }

        [AllowAnonymous]
        [HttpPost("SignupAsaUser")]
        public async Task<IActionResult> SignupAsaPatient([FromForm] SignupAsPatientDto model)
        {
            if (!ModelState.IsValid) return BadRequest();
            string? imagePath = model.Image != null ? await _imageService.SaveImageAsync(model.Image) : null;
            var result = await _accountRepo.SignupAsPatientAsync(model, imagePath);
            if (result.Succeeded) return Ok("Registered successful");
            return StatusCode(500, new { errors = result.Errors.Select(e => e.Description) });
        }
        [Authorize(Roles = "Admin,subadmin")]
        [HttpPost("SignupAsADoctor")]
        public async Task<IActionResult> SignupAsADoctor([FromForm] SignupOfDoctorDto model)
        {
            if (!ModelState.IsValid) return BadRequest();
            string? imagePath = model.Image != null ? await _imageService.SaveImageAsync(model.Image) : null;
            var result = await _accountRepo.SignupAsDoctorAsync(model, imagePath);
            if (result.Succeeded) return Ok("Registered successful");
            return StatusCode(500, new { errors = result.Errors.Select(e => e.Description) });
        }

        [Authorize(Roles = "Admin,subadmin")]
        [HttpPost("SignupAsNurse")]
        public async Task<IActionResult> SignupAsNurse([FromForm] SignupOfNurseDto model)
        {
            if (!ModelState.IsValid) return BadRequest();
            string? imagePath = model.Image != null ? await _imageService.SaveImageAsync(model.Image) : null;
            var result = await _accountRepo.SignupAsNurseAsync(model, imagePath);
            if (result.Succeeded) return Ok("Registered successful");
            return StatusCode(500, new { errors = result.Errors.Select(e => e.Description) });
        }

        [Authorize(Roles = "Admin,subadmin")]
        [HttpPost("SignupAsSatff")]
        public async Task<IActionResult> SignupAsStaff([FromForm] SignupOfStaffDto model)
        {
            if (!ModelState.IsValid) return BadRequest();
            string? imagePath = model.Image != null ? await _imageService.SaveImageAsync(model.Image) : null;
            var result = await _accountRepo.SignupAsStaffAsync(model, imagePath);
            if (result.Succeeded) return Ok("Registered successful");
            return StatusCode(500, new { errors = result.Errors.Select(e => e.Description) });
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            if (!ModelState.IsValid) return BadRequest();
            var response = await _accountRepo.LoginAsync(login);
            if (response == null || !response.IsAuthenticated) return StatusCode(500, "password or/and email isn't true or account is deactivated");

            if (!string.IsNullOrEmpty(response.RefreshToken))
            {
                _tokenServices.SetRefreshTokenInCookies(response.RefreshToken, response.RefreshTokenExpiration);
            }

            return Ok(response);
        }
        [Authorize]
        [HttpGet("GetDoctors")]
        public async Task<IActionResult> GetDoctors() => Ok(await _accountRepo.GetDoctorsAsync());

        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var response = await _accountRepo.RefreshTokenAsync();
            if (response == null || !response.IsAuthenticated)
                return BadRequest("Invalid token");

            _tokenServices.SetRefreshTokenInCookies(response.RefreshToken!, response.RefreshTokenExpiration);
            return Ok(response);
        }
        [Authorize]
        [HttpGet("GetDoctors/{id}")]
        public async Task<IActionResult> GetDoctorById(string id)
        {
            var result = await _accountRepo.GetDoctorByIdAsync(id);
            if (result == null) return NotFound("Doctor not found");
            return Ok(result);
        }

        [Authorize(Roles = "Admin,subadmin")]
        [HttpGet("GetStaff")]
        public async Task<IActionResult> GetStaff() => Ok(await _accountRepo.GetStaffAsync());
        [Authorize(Roles = "Admin,subadmin")]

        [Authorize(Roles = "Admin,subadmin")]
        [HttpGet("GetStaff/{id}")]
        public async Task<IActionResult> GetStaffById(string id)
        {
            var result = await _accountRepo.GetStaffByIdAsync(id);
            if (result == null) return NotFound("Staff not found");
            return Ok(result);
        }

        [Authorize(Roles = "Admin,subadmin")]
        [HttpGet("GetNurses")]
        public async Task<IActionResult> GetNurses() => Ok(await _accountRepo.GetNursesAsync());

        [Authorize(Roles = "Admin,subadmin")]
        [HttpGet("GetNurses/{id}")]
        public async Task<IActionResult> GetNurseById(string id)
        {
            var result = await _accountRepo.GetNurseByIdAsync(id);
            if (result == null) return NotFound("Nurse not found");
            return Ok(result);
        }
        [Authorize(Roles = "Admin,subadmin")]
        [HttpGet("GetPatients")]
        public async Task<IActionResult> GetPatients() => Ok(await _accountRepo.GetPatientsAsync());

        [Authorize(Roles = "Staff,Doctor")]
        [HttpGet("GetPatientByIdOrName/{id}/{name}")]
        public async Task<IActionResult> GetPatientByIdOrName(int id, string name)
        {
            var result = await _accountRepo.GetPatientByIdOrNameAsync(id, name);
            if (result == null) return NotFound("Patient not found");
            return Ok(result);
        }
        [Authorize(Roles = "Admin,Doctor,subadmin")]
        [HttpGet("GetPatients/{id}")]
        public async Task<IActionResult> GetPatientById(string id)
        {
            var result = await _accountRepo.GetPatientByIdAsync(id);
            if (result == null) return NotFound("Patient not found");
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetSubAdmins")]
        public async Task<IActionResult> GetSubAdmins() => Ok(await _accountRepo.GetSubAdminsAsync());

        [Authorize(Roles = "Admin")]
        [HttpGet("GetSubAdmins/{id}")]
        public async Task<IActionResult> GetSubAdminById(string id)
        {
            var result = await _accountRepo.GetSubAdminByIdAsync(id);
            if (result == null) return NotFound("SubAdmin not found");
            return Ok(result);
        }

        [Authorize]
        [HttpPost("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            var result = await _accountRepo.LogOutAsync();
            if (result) return Ok("Log Out successfully");
            return BadRequest("Could not log out");
        }

        [Authorize(Roles = "Admin,subadmin")]
        [HttpDelete("doctor/{userId}")]
        public async Task<IActionResult> DeleteDoctor(string userId)
        {
            if (await _accountRepo.DeleteDoctorAsync(userId)) return Ok("Doctor soft-deleted successfully");
            return NotFound("Doctor not found");
        }

        [Authorize(Roles = "Admin,subadmin")]
        [HttpDelete("patient/{userId}")]
        public async Task<IActionResult> DeletePatient(string userId)
        {
            if (await _accountRepo.DeletePatientAsync(userId)) return Ok("Patient soft-deleted successfully");
            return NotFound("Patient not found");
        }

        [Authorize(Roles = "Admin,subadmin")]
        [HttpDelete("staff/{userId}")]
        public async Task<IActionResult> DeleteStaff(string userId)
        {
            if (await _accountRepo.DeleteStaffAsync(userId)) return Ok("Staff soft-deleted successfully");
            return NotFound("Staff not found");
        }

        [Authorize(Roles = "Admin,subadmin")]
        [HttpDelete("nurse/{userId}")]
        public async Task<IActionResult> DeleteNurse(string userId)
        {
            if (await _accountRepo.DeleteNurseAsync(userId)) return Ok("Nurse soft-deleted successfully");
            return NotFound("Nurse not found");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("subadmin/{userId}")]
        public async Task<IActionResult> DeleteSubAdmin(string userId)
        {
            if (await _accountRepo.DeleteSubAdminAsync(userId)) return Ok("SubAdmin soft-deleted successfully");
            return NotFound("SubAdmin not found");
        }

        [Authorize(Roles = "Admin,subadmin")]
        [HttpPost("ReturnFromDelete/{userId}")]
        public async Task<IActionResult> ReturnFromDelete(string userId)
        {
            if (await _accountRepo.ReturnFromDeleteAsync(userId)) return Ok("Account returned from delete successfully");
            return NotFound("User not found");
        }
        [Authorize(Roles = "Admin,SubAdmin")]
        [HttpPost("deactivate/{userId}")]
        public async Task<IActionResult> DeactivateAccount(string userId)
        {
            if (await _accountRepo.DeactivateAccountAsync(userId)) return Ok("Account deactivated successfully");
            return NotFound("User not found");
        }
        [Authorize(Roles = "Admin,SubAdmin")]
        [HttpPost("activate/{userId}")]
        public async Task<IActionResult> ActivateAccount(string userId)
        {
            if (await _accountRepo.ActivateAccountAsync(userId)) return Ok("Account activated successfully");
            return NotFound("User not found");
        }
        [Authorize(Roles = "Admin,SubAdmin")]
        [HttpGet("deleted-doctors")]
        public async Task<IActionResult> GetDeletedDoctors() => Ok(await _accountRepo.GetDeletedDoctorsAsync());

        [Authorize(Roles = "Admin,SubAdmin")]
        [HttpGet("deleted-nurses")]
        public async Task<IActionResult> GetDeletedNurses() => Ok(await _accountRepo.GetDeletedNursesAsync());

        [Authorize(Roles = "Admin,SubAdmin")]
        [HttpGet("deleted-patients")]
        public async Task<IActionResult> GetDeletedPatients() => Ok(await _accountRepo.GetDeletedPatientsAsync());

        [Authorize(Roles = "Admin,SubAdmin")]
        [HttpGet("deactivated-users")]
        public async Task<IActionResult> GetDeactivatedUsers() => Ok(await _accountRepo.GetDeactivatedUsersAsync());

        [Authorize(Roles = "Patient")]
        [HttpPut("UpdatePatientProfile")]
        public async Task<IActionResult> UpdatePatientProfile([FromForm] UPdatePatientProfileDto model)
        {
            var userId = await _accountRepo.GetPatientByIdAsync(model.Id);
            if (userId == null) return Unauthorized();

            string? imagePath = model.Image != null ? await _imageService.SaveImageAsync(model.Image) : null;
            var result = await _accountRepo.UpdatePatientProfileAsync(model, imagePath);

            if (result.Succeeded) return Ok("Profile updated successfully");
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        [Authorize(Roles = "Doctor")]
        [HttpPut("UpdateDoctorProfile")]
        public async Task<IActionResult> UpdateDoctorProfile([FromForm] UpdateDoctorProfileDto model)
        {
            var userId = await _accountRepo.GetDoctorByIdAsync(model.Id);
            if (userId == null) return Unauthorized();

            string? imagePath = model.Image != null ? await _imageService.SaveImageAsync(model.Image) : null;
            var result = await _accountRepo.UpdateDoctorProfileAsync(model, imagePath);

            if (result.Succeeded) return Ok("Profile updated successfully");
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        [Authorize(Roles = "Staff")]
        [HttpPut("UpdateStaffProfile")]
        public async Task<IActionResult> UpdateStaffProfile([FromForm] UpdateStaffProfileDto model)
        {
            var userId = await _accountRepo.GetStaffByIdAsync(model.Id);
            if (userId == null) return Unauthorized();

            string? imagePath = model.Image != null ? await _imageService.SaveImageAsync(model.Image) : null;
            var result = await _accountRepo.UpdateStaffProfileAsync(model, imagePath);

            if (result.Succeeded) return Ok("Profile updated successfully");
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        [Authorize(Roles = "Nurse")]
        [HttpPut("UpdateNurseProfile")]
        public async Task<IActionResult> UpdateNurseProfile([FromForm] UpdateNurseProfileDto model)
        {
            var userId = await _accountRepo.GetNurseByIdAsync(model.Id);
            if (userId == null) return Unauthorized();

            string? imagePath = model.Image != null ? await _imageService.SaveImageAsync(model.Image) : null;
            var result = await _accountRepo.UpdateNurseProfileAsync(model, imagePath);

            if (result.Succeeded) return Ok("Profile updated successfully");
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateAdminProfile")]
        public async Task<IActionResult> UpdateAdminProfile([FromForm] UpdateAdminProfileDto model)
        {
            var userId = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.Id);
            if (userId == null) return Unauthorized();

            string? imagePath = model.Image != null ? await _imageService.SaveImageAsync(model.Image) : null;
            var result = await _accountRepo.UpdateAdminProfileAsync(model, imagePath);

            if (result.Succeeded) return Ok("Profile updated successfully");
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        [Authorize(Roles = "Doctor,Admin")]
        [HttpGet("GetUserHistory")]
        public async Task<IActionResult> GetUserHistory(string userId)
        {
            var result = await _accountRepo.GetUserHistoryAsync(userId);
            if (result == null) return NotFound("User not found");
            return Ok(result);
        }

        [Authorize(Roles = "Doctor")]
        [HttpPut("UpdatepressureAndSugarOfPatient")]
        public async Task<IActionResult> UpdatepressureAndSugarOfPatient([FromBody] UpdatePressureAndSugarOfPatientDto model)
        {
            var result = await _accountRepo.UpdatepressureAndSugarOfPatientAsync(model);
            if (result == null) return NotFound("User not found");
            return Ok(result);
        }
    }
}