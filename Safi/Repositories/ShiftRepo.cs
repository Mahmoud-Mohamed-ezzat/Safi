using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Safi.Dto.Account;
using Safi.Dto.AssignRoomToDoctorDto;
using Safi.Dto.ShiftDto;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;

namespace Safi.Repositories
{
    public class ShiftRepo : IShift
    {
        private readonly SafiContext _context;
        private readonly UserManager<User> _userManager;

        public ShiftRepo(SafiContext context, UserManager<User> userManaer)
        {
            _context = context;
            _userManager = userManaer;
        }

        // CRUD
        public async Task<List<ShiftDto>> GetAllAsync()
        {
            var shifts = await _context.Shifts.AsNoTracking().ToListAsync();
            return shifts.Select(s => s.ToShiftDto()).ToList();
        }

        public async Task<ShiftDto?> GetByIdAsync(int id)
        {
            var shift = await _context.Shifts.FindAsync(id);
            return shift?.ToShiftDto();
        }

        public async Task<ShiftDto> CreateAsync(CreateShiftDto dto)
        {
            var shift = dto.ToShift();
            var same_shift=await _context.Shifts.FirstOrDefaultAsync(s=>s.StartTime==shift.StartTime && s.EndTime==shift.EndTime);
            if (same_shift != null) return null;
            await _context.Shifts.AddAsync(shift);
            await _context.SaveChangesAsync();
            return shift.ToShiftDto();
        }

        public async Task<ShiftDto?> UpdateAsync(int id, UpdateShiftDto dto)
        {
            var shift = await _context.Shifts.FindAsync(id);
            if (shift == null) return null;
            if (dto.StartTime.HasValue) shift.StartTime = dto.StartTime.Value;
            if (dto.EndTime.HasValue) shift.EndTime = dto.EndTime.Value;
            var same_shift = await _context.Shifts.FirstOrDefaultAsync(s => s.StartTime == shift.StartTime && s.EndTime == shift.EndTime);
            if(same_shift!=null) return null;
            await _context.SaveChangesAsync();
            return shift.ToShiftDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var shift = await _context.Shifts.FindAsync(id);
            if (shift == null) return false;

            _context.Shifts.Remove(shift);
            await _context.SaveChangesAsync();
            return true;
        }

        // Specific Retrieval
        public async Task<List<GetDoctorsDto>> GetDoctorsByShiftIdAsync(int shiftId)
        {
            var assignments = await _context.AssignRoomToDoctors
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Department)
                .Where(a => a.ShiftId == shiftId)
                .AsNoTracking()
                .ToListAsync();

            // Select distinct doctors
            var doctors = assignments
                .Select(a => a.Doctor!)
                .DistinctBy(d => d.Id)
                .Select(d => d.ToGetDoctorsDto())
                .ToList();

            return doctors;
        }

        public async Task<GetDoctorsDto?> GetDoctorByShiftIdAsync(int shiftId, string doctorId)
        {
            // Find if doctor is assigned in this shift
            var assignment = await _context.AssignRoomToDoctors
               .Include(a => a.Doctor)
               .ThenInclude(d => d.Department)
               .Where(a => a.ShiftId == shiftId && a.DoctorId == doctorId)
               .AsNoTracking()
               .FirstOrDefaultAsync();

            return assignment?.Doctor?.ToGetDoctorsDto();
        }

        public async Task<List<AssignRoomToDoctorDto>> GetAssignmentsByShiftIdAsync(int shiftId)
        {
            var assignments = await _context.AssignRoomToDoctors
               .Include(a => a.Room)
               .Include(a => a.Doctor)
               .Include(a => a.Shift)
               .Where(a => a.ShiftId == shiftId)
               .AsNoTracking()
               .ToListAsync();

            return assignments.Select(a => a.ToAssignRoomToDoctorDto()).ToList();
        }

        public async Task<AssignRoomToDoctorDto?> GetAssignmentByShiftIdAndRoomIdAsync(int shiftId, int roomId)
        {
            var assignment = await _context.AssignRoomToDoctors
                .Include(a => a.Room)
                .Include(a => a.Doctor)
                .Include(a => a.Shift)
                .Where(a => a.ShiftId == shiftId && a.RoomId == roomId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return assignment?.ToAssignRoomToDoctorDto();
        }

        public async Task<List<GetDoctorsDto>> GetDoctorsAtDateByShiftIdAsync(int shiftId, DateOnly date)
        {
            var assigns = await _context.AssignRoomToDoctors.Include(a => a.Doctor).Include(a => a.Shift).Where(a => a.ShiftId == shiftId && a.EndDate >= date && a.StartDate <= date).ToListAsync();
            if (assigns == null) return null;
            var Doctors = assigns.Select(a => a.Doctor.ToGetDoctorsDto()).ToList();
            if (Doctors == null) return null;
            return Doctors;
        }

        public async Task<List<GetDoctorsshiftDto>> GetDoctorsAtDateinroomByShiftIdAsync(int shiftId, DateOnly date, int roomId)
        {
            var assigns = await _context.AssignRoomToDoctors.AsNoTracking().Include(a => a.Doctor).Include(a => a.Shift).Where(a => a.ShiftId == shiftId && a.RoomId == roomId && a.EndDate >= date && a.StartDate <= date).ToListAsync();
            if (assigns == null) return null;
            var Doctors = assigns.Select(a => a.Doctor.toGetDoctorsshiftDto(roomId)).ToList();
            if (Doctors == null) return null;
            return Doctors;

        }

        public async Task<GetDoctorsDto?> GetDoctorAtDateByShiftIdAsync(int shiftId, DateOnly date, string doctorId)
        {
            var doctor = await _userManager.FindByIdAsync(doctorId);
            if (doctor == null) return null;
            var assign= await _context.AssignRoomToDoctors.Include(a=>a.Doctor).Include(a=>a.Room).FirstOrDefaultAsync(a=>a.ShiftId==shiftId&&a.EndDate>=date&&a.StartDate<=a.StartDate&&a.DoctorId==doctorId); 
            if (assign == null) return null;
            return assign?.Doctor?.ToGetDoctorsDto();
        }

        public async Task<GetDoctorsDto?> GetDoctorAtDateinroomByShiftIdAsync(int shiftId, DateOnly date, int roomId, string doctorId)
        {
            var doctor = await _userManager.FindByIdAsync(doctorId);
            if (doctor == null) return null;
            var assign= await _context.AssignRoomToDoctors.Include(a=>a.Doctor).Include(a=>a.Room).FirstOrDefaultAsync(a=>a.ShiftId==shiftId&&a.EndDate>=date&&a.StartDate<=a.StartDate&&a.DoctorId==doctorId&&a.RoomId==roomId); 
            if (assign == null) return null;
            return assign?.Doctor?.ToGetDoctorsDto();
        }

        public async Task<List<AssignRoomToDoctorDto>> GetAssignmentsAtDateByShiftIdAsync(int shiftId, DateOnly date)
        {
            var assigns = await _context.AssignRoomToDoctors.Include(a => a.Doctor).Include(a => a.Shift).Where(a => a.ShiftId == shiftId && a.EndDate >= date && a.StartDate <= date).ToListAsync();
            if (assigns == null) return null;
            var Doctors = assigns.Select(a => a.ToAssignRoomToDoctorDto()).ToList();
            if (Doctors == null) return null;
            return Doctors;
        }

        public async Task<List<AssignRoomToDoctorDto>> GetAssignmentsAtDateinroomByShiftIdAsync(int shiftId, DateOnly date, int roomId)
        {
            var assigns = await _context.AssignRoomToDoctors.Include(a => a.Doctor).Include(a => a.Shift).Where(a => a.ShiftId == shiftId && a.EndDate >= date && a.StartDate <= date&&a.RoomId==roomId).ToListAsync();
            if (assigns == null) return null;
            var Doctors = assigns.Select(a => a.ToAssignRoomToDoctorDto()).ToList();
            if (Doctors == null) return null;
            return Doctors;
        }
    }
}
