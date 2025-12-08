using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Safi.Dto.Account;
using Safi.Mapper;
using Safi.Models;
using Safi.Services;

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
        readonly SafiContext _context;
        public AccountsController(SafiContext context, TokenService tokenServices, UserManager<User> userManager, SignInManager<User> signInManager, ImageService imageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenServices = tokenServices;
            _imageService = imageService;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("SignUpSubAdmin")]
        public async Task<IActionResult> signupAsSubAdmin([FromForm] SignupFoRAdminOfWebDto model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest();

                string imagePath = null;
                if (model.Image != null && model.Image.Length > 0)
                {
                    imagePath = await _imageService.SaveImageAsync(model.Image);
                }

                var user = new User
                {
                    Name = model.username,
                    UserName = model.email,
                    Email = model.email,
                    PhoneNumber = model.Phone,
                    Image = imagePath
                };
                var createduser = await _userManager.CreateAsync(user, model.Password);
                if (createduser.Succeeded)
                {
                    var Role = await _userManager.AddToRoleAsync(user, "SubAdmin");
                    if (Role.Succeeded)
                    {

                        return Ok($"registerd is successful ");
                    }
                    else { return StatusCode(500, new { errors = Role.Errors.Select(e => e.Description) }); }
                }
                else { return StatusCode(500, new { errors = createduser.Errors.Select(e => e.Description) }); }
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(new { error = ex.Message, innerError = innerMessage, stackTrace = ex.InnerException?.StackTrace });
            }
        }

        [AllowAnonymous]
        [HttpPost("SignUpAdmin")]
        public async Task<IActionResult> signupAsAdmin([FromForm] SignupFoRAdminOfWebDto model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest();

                string imagePath = null;
                if (model.Image != null && model.Image.Length > 0)
                {
                    imagePath = await _imageService.SaveImageAsync(model.Image);
                }

                var user = new User
                {
                    Name = model.username,
                    UserName = model.email,
                    Email = model.email,
                    PhoneNumber = model.Phone,
                    Image = imagePath
                };
                var createduser = await _userManager.CreateAsync(user, model.Password);
                if (createduser.Succeeded)
                {
                    var Role = await _userManager.AddToRoleAsync(user, "Admin");
                    if (Role.Succeeded)
                    {

                        return Ok($"registerd is successful ");
                    }
                    else { return StatusCode(500, new { errors = Role.Errors.Select(e => e.Description) }); }
                }
                else { return StatusCode(500, new { errors = createduser.Errors.Select(e => e.Description) }); }
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(new { error = ex.Message, innerError = innerMessage, stackTrace = ex.InnerException?.StackTrace });
            }
        }

        [AllowAnonymous]
        [HttpPost("SignupAsaUser")]
        public async Task<IActionResult> SignupAsaPatient([FromForm] SignupAsPatientDto model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest();

                string imagePath = null;
                if (model.Image != null && model.Image.Length > 0)
                {
                    imagePath = await _imageService.SaveImageAsync(model.Image);
                }

                var user = new Patient
                {
                    Name = model.username,
                    UserName = model.email,
                    Email = model.email,
                    PhoneNumber = model.Phone,
                    Image = imagePath,
                    HasSugar = model.hassugar,
                    HasPressure = model.hasPressure
                };
                var createduser = await _userManager.CreateAsync(user, model.Password);
                if (createduser.Succeeded)
                {
                    var Role = await _userManager.AddToRoleAsync(user, "Patient");
                    if (Role.Succeeded)
                    {

                        return Ok($"registerd is successful ");
                    }
                    else { return StatusCode(500, new { errors = Role.Errors.Select(e => e.Description) }); }
                }
                else { return StatusCode(500, new { errors = createduser.Errors.Select(e => e.Description) }); }
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(new { error = ex.Message, innerError = innerMessage, stackTrace = ex.InnerException?.StackTrace });
            }
        }
        [AllowAnonymous]
        [HttpPost("SignupAsAPublisher")]
        public async Task<IActionResult> SignupAsADoctor([FromForm] SignupOfDoctorDto model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest();

                string imagePath = null;
                if (model.Image != null && model.Image.Length > 0)
                {
                    imagePath = await _imageService.SaveImageAsync(model.Image);
                }

                var user = new Doctor
                {
                    Name = model.username,
                    UserName = model.email,
                    Email = model.email,
                    PhoneNumber = model.Phone,
                    Image = imagePath,
                    University = model.University,
                    Degree = model.Degree,
                    Rank = model.Rank,
                    DepartmentId = model.DepartmentId
                };
                var createduser = await _userManager.CreateAsync(user, model.Password);
                if (createduser.Succeeded)
                {
                    var Role = await _userManager.AddToRoleAsync(user, "Doctor");
                    if (Role.Succeeded)
                    {

                        return Ok($"registerd is successful ");
                    }
                    else { return StatusCode(500, new { errors = Role.Errors.Select(e => e.Description) }); }
                }
                else { return StatusCode(500, new { errors = createduser.Errors.Select(e => e.Description) }); }
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(new { error = ex.Message, innerError = innerMessage, stackTrace = ex.InnerException?.StackTrace });
            }
        }

        [AllowAnonymous]
        [HttpPost("SignupAsSatff")]
        public async Task<IActionResult> SignupAsStaff([FromForm] SignupOfStaffDto model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest();

                string imagePath = null;
                if (model.Image != null && model.Image.Length > 0)
                {
                    imagePath = await _imageService.SaveImageAsync(model.Image);
                }

                var user = new Staff
                {
                    Name = model.username,
                    UserName = model.email,
                    Email = model.email,
                    PhoneNumber = model.Phone,
                    Image = imagePath,
                    University = model.University,
                    DepartmentId = model.DepartmentId
                };
                var createduser = await _userManager.CreateAsync(user, model.Password);
                if (createduser.Succeeded)
                {
                    var Role = await _userManager.AddToRoleAsync(user, "Staff");
                    if (Role.Succeeded)
                    {

                        return Ok($"registerd is successful ");
                    }
                    else { return StatusCode(500, new { errors = Role.Errors.Select(e => e.Description) }); }
                }
                else { return StatusCode(500, new { errors = createduser.Errors.Select(e => e.Description) }); }
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(new { error = ex.Message, innerError = innerMessage, stackTrace = ex.InnerException?.StackTrace });
            }
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            if (!ModelState.IsValid) return BadRequest();
            var user = await _userManager.FindByEmailAsync(login.Email);
            var model = await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);
            var VerifiedUser = await _userManager.IsEmailConfirmedAsync(user);
            if (!model.Succeeded && !VerifiedUser) return StatusCode(500, "password or/and email isn't true");
            return Ok(_tokenServices.GenerateToken(user));
        }
        [Authorize]
        [HttpGet("GetDoctors")]
        public async Task<IActionResult> GetDoctors()
        {
            var Doctors = await _context.Doctors.AsNoTracking()
                .Include(d => d.Department).ToListAsync() ?? throw new Exception("Doctors not found");

            var DoctorsDto = Doctors.Select(p => p.ToGetDoctorsDto()).ToList();
            return Ok(DoctorsDto);
        }
        [Authorize]
        [HttpGet("GetDoctors/id")]
        public async Task<IActionResult> GetDoctorById(string id)
        {
            var Doctor = await _context.Doctors.AsNoTracking().Include(d => d.Department).FirstOrDefaultAsync(d => d.Id == id)
            ?? throw new Exception("Doctor not found");

            var DoctorsDto = Doctor.ToGetDoctorsDto();
            return Ok(DoctorsDto);
        }
        [Authorize]
        [HttpGet("GetStaff")]
        public async Task<IActionResult> GetStaff()
        {
            var Staffs = await _context.Staffs.AsNoTracking()
                .Include(d => d.Department).ToListAsync()
                ?? throw new Exception("Staff not found");

            var StaffDto = Staffs.Select(p => p.ToGetStaffsDto()).ToList();
            return Ok(StaffDto);
        }
        [Authorize]
        [HttpGet("GetStaff/id")]
        public async Task<IActionResult> GetStaffById(string id)
        {
            var Staff = await _context.Staffs.AsNoTracking()
                .Include(d => d.Department).FirstOrDefaultAsync(d => d.Id == id)
                ?? throw new Exception("Staff not found");

            var StaffDto = Staff.ToGetStaffsDto();
            return Ok(StaffDto);
        }
        [Authorize]
        [HttpGet("GetPatients")]
        public async Task<IActionResult> GetPatients()
        {
            var Patients = await _context.Patients.AsNoTracking()
                .Include(d => d.Departments).ToListAsync()
                ?? throw new Exception("Patients not found");

            var PatientsDto = Patients.Select(p => p.ToGetPatientsDto()).ToList();
            return Ok(PatientsDto);
        }
        [Authorize]
        [HttpGet("GetPatients/id")]
        public async Task<IActionResult> GetPatientById(string id)
        {
            var Patient = await _context.Patients.AsNoTracking()
                .Include(d => d.Departments).FirstOrDefaultAsync(d => d.Id == id)
                ?? throw new Exception("Patient not found");

            var PatientDto = Patient.ToGetPatientsDto();
            return Ok(PatientDto);
        }
        [Authorize]
        [HttpGet("GetSubAdmins")]
        public async Task<IActionResult> GetSubAdmins()
        {
            var SubAdmins = await _userManager.GetUsersInRoleAsync("SubAdmin");
            var SubAdminsDto = SubAdmins.Select(s => s.ToGetSubAdminsDto()).ToList();
            return Ok(SubAdminsDto);
        }
        [Authorize]
        [HttpGet("GetSubAdmins/id")]
        public async Task<IActionResult> GetSubAdminById(string id)
        {
            var subAdmins = await _userManager.GetUsersInRoleAsync("SubAdmin");
            var SubAdmin = subAdmins.FirstOrDefault(d => d.Id == id) ?? throw new Exception("SubAdmin not found");
            var SubAdminDto = SubAdmin.ToGetSubAdminsDto();
            return Ok(SubAdminDto);
        }
        [Authorize]
        [HttpPost("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return BadRequest("not found");
            var refreshtoken = await _tokenServices.RevokeRefreshToken();
            await _userManager.UpdateSecurityStampAsync(user);
            return Ok("log Out succesfully");
        }

    }
}
