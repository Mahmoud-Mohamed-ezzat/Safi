using Microsoft.EntityFrameworkCore;
using Safi.Dto.Attendance;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;

namespace Safi.Repositories
{
    public class AttendanceRepo : IAttendance
    {
        readonly SafiContext _context;
        public AttendanceRepo(SafiContext context)
        {
            this._context = context;
        }

        public async Task<GetAttendanceDto> CreateAttendanceAsync(CreateAttendanceDto dto)
        {
            var attendance = dto.ToCreateAttendance();
            if (dto.Date < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new InvalidOperationException("Attendance date cannot be in the past");
            }
            await _context.Attendance.AddAsync(attendance);
            await _context.SaveChangesAsync();
            await _context.Entry(attendance).Reference(a => a.Doctor).LoadAsync();
            await _context.Entry(attendance).Reference(a => a.DoctorShift).LoadAsync();
            return attendance.ToGetAttendanceDto();
        }

        public async Task<bool> DeleteAttendanceAsync(int id)
        {
            var attendance = await _context.Attendance.FirstOrDefaultAsync(a => a.Id == id);
            if (attendance != null)
            {
                _context.Attendance.Remove(attendance);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<GetAttendanceDto>> GetAllAsync()
        {
            var attendances = await _context.Attendance.Include(a => a.Doctor).Include(a => a.DoctorShift).AsNoTracking().ToListAsync();
            return attendances.Select(a => a.ToGetAttendanceDto()).ToList();
        }

        public async Task<List<GetAttendanceDto?>> GetAllByDateAsync(DateOnly Day)
        {
            var attendances = await _context.Attendance.Include(a => a.Doctor).Include(a => a.DoctorShift).Where(a => a.Date == Day).AsNoTracking().ToListAsync();
            return attendances.Select(a => (GetAttendanceDto?)a.ToGetAttendanceDto()).ToList();
        }

        public async Task<List<GetAttendanceDto?>> GetAllTodayAsync()
        {
            var attendances = await _context.Attendance.Include(a => a.Doctor).Include(a => a.DoctorShift).Where(a => a.Date == DateOnly.FromDateTime(DateTime.UtcNow)).AsNoTracking().ToListAsync();
            return attendances.Select(a => (GetAttendanceDto?)a.ToGetAttendanceDto()).ToList();
        }

        public async Task<List<GetAttendanceDto?>> GetByIdAndDateOfDoctorAsync(string id, DateOnly Day)
        {
            var attendances = await _context.Attendance.Include(a => a.Doctor).Include(a => a.DoctorShift).Where(a => a.Date == Day && a.Doctor.Id == id).AsNoTracking().ToListAsync();
            return attendances.Select(a => (GetAttendanceDto?)a.ToGetAttendanceDto()).ToList();
        }

        public async Task<GetAttendanceDto?> GetByIdAsync(int id)
        {
            var attendance = await _context.Attendance
                .Include(a => a.Doctor)
                .Include(a => a.DoctorShift)
                .Where(a => a.Id == id).AsNoTracking().FirstOrDefaultAsync();
            return attendance?.ToGetAttendanceDto();
        }

        public async Task<List<GetAttendanceDto?>> GetByIdOfDoctorAsync(string id)
        {
            var attendances = await _context.Attendance
                .Include(a => a.Doctor)
                .Include(a => a.DoctorShift)
                .Where(a => a.Doctor.Id == id).AsNoTracking().ToListAsync();
            return attendances.Select(a => (GetAttendanceDto?)a.ToGetAttendanceDto()).ToList();
        }

        public async Task<GetAttendanceDto> UpdateAttendanceAsync(int id, UpdateAttendanceDto dto)
        {
            var attendance = await _context.Attendance
                .Include(a => a.Doctor)
                .Include(a => a.DoctorShift)
                .FirstOrDefaultAsync(a => a.Id == id);
            
            if (attendance == null) throw new InvalidOperationException("Attendance not found");
            
            attendance.Notes = dto.Notes ?? attendance.Notes;
            attendance.status = dto.status ?? attendance.status;

            await _context.SaveChangesAsync();
            return attendance.ToGetAttendanceDto();
        }
    }
}
