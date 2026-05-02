using Microsoft.EntityFrameworkCore;
using Safi.Dto.AssignWorksDto;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;

namespace Safi.Repositories
{
    public class AssignWorksRepo : IAssignWorks
    {
        private readonly SafiContext _context;

        public AssignWorksRepo(SafiContext context)
        {
            _context = context;
        }

        public async Task<List<AssignWorksDto>> GetAllAsync()
        {
            var assignments = await _context.AssignWorks
                .Include(a => a.Room)
                .Include(a => a.user)
                .Include(a => a.Shift)
                .AsNoTracking()
                .ToListAsync();

            return assignments.Select(a => a.ToAssignRoomToDoctorDto()).ToList();
        }

        public async Task<AssignWorksDto?> GetByIdAsync(int id)
        {
            var assignment = await _context.AssignWorks
                .Include(a => a.Room)
                .Include(a => a.user)
                .Include(a => a.Shift)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            return assignment?.ToAssignRoomToDoctorDto();
        }

        public async Task<AssignWorksDto> CreateAsync(CreateAssignWorksDto dto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.DoctorId);
                var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == dto.RoomId);
                var shift = await _context.Shifts.FirstOrDefaultAsync(s => s.Id == dto.ShiftId);

                if (user == null)
                {
                    throw new InvalidOperationException($"User with ID {dto.DoctorId} not found.");
                }

                if (room == null)
                {
                    throw new InvalidOperationException($"Room with ID {dto.RoomId} not found.");
                }

                if (shift == null)
                {
                    throw new InvalidOperationException($"Shift with ID {dto.ShiftId} not found.");
                }

                // If it's a doctor, validate department match
                if (user is Doctor doctor)
                {
                    if (doctor.DepartmentId != room.DepartmentId)
                    {
                        throw new InvalidOperationException($"Doctor belongs to department {doctor.DepartmentId}, but room {dto.RoomId} belongs to department {room.DepartmentId}.");
                    }
                }
                else if (user is Nurse nurse)
                {
                    if (nurse.DepartmentId != room.DepartmentId)
                    {
                        throw new InvalidOperationException($"Nurse belongs to department {nurse.DepartmentId}, but room {dto.RoomId} belongs to department {room.DepartmentId}.");
                    }
                }
                else if (user is Staff staff)
                {
                    if (staff.DepartmentId != room.DepartmentId)
                    {
                        throw new InvalidOperationException($"Staff belongs to department {staff.DepartmentId}, but room {dto.RoomId} belongs to department {room.DepartmentId}.");
                    }
                }

                // Check if this room is already assigned to the same user
                var existingAssignment = await _context.AssignWorks
                    .FirstOrDefaultAsync(a => a.RoomId == dto.RoomId && a.userId == dto.DoctorId);

                if (existingAssignment != null)
                {
                    throw new InvalidOperationException($"Room {dto.RoomId} is already assigned to user {dto.DoctorId}.");
                }

                var assignment = dto.ToCreateAssignRoomToDoctorDto();
                await _context.AssignWorks.AddAsync(assignment);
                await _context.SaveChangesAsync();

                // Load navigation properties
                await _context.Entry(assignment).Reference(a => a.Room).LoadAsync();
                await _context.Entry(assignment).Reference(a => a.user).LoadAsync();
                await _context.Entry(assignment).Reference(a => a.Shift).LoadAsync();

                return assignment.ToAssignRoomToDoctorDto();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create assignment: {ex.Message}", ex);
            }
        }

        public async Task<AssignWorksDto?> UpdateAsync(int id, UpdateAssignWorksDto dto)
        {
            try
            {
                var assignment = await _context.AssignWorks.FirstOrDefaultAsync(a => a.Id == id);
                if (assignment == null) return null;

                if (dto.RoomId.HasValue) assignment.RoomId = dto.RoomId.Value;
                if (!string.IsNullOrEmpty(dto.DoctorId)) assignment.userId = dto.DoctorId;

                await _context.SaveChangesAsync();

                await _context.Entry(assignment).Reference(a => a.Room).LoadAsync();
                await _context.Entry(assignment).Reference(a => a.user).LoadAsync();
                await _context.Entry(assignment).Reference(a => a.Shift).LoadAsync();

                return assignment.ToAssignRoomToDoctorDto();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to update assignment: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var assignment = await _context.AssignWorks.FirstOrDefaultAsync(a => a.Id == id);
            if (assignment == null) return false;

            _context.AssignWorks.Remove(assignment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<AssignWorksDto>> GetByDoctorIdAsync(string doctorId)
        {
            var assignments = await _context.AssignWorks
                .Include(a => a.Room)
                .Include(a => a.user)
                .Include(a => a.Shift)
                .Where(a => a.userId == doctorId)
                .AsNoTracking()
                .ToListAsync();

            return assignments.Select(a => a.ToAssignRoomToDoctorDto()).ToList();
        }

        public async Task<List<AssignWorksDto>> GetByRoomIdAsync(int roomId)
        {
            var assignments = await _context.AssignWorks
                .Include(a => a.Room)
                .Include(a => a.user)
                .Include(a => a.Shift)
                .Where(a => a.RoomId == roomId)
                .AsNoTracking()
                .ToListAsync();

            return assignments.Select(a => a.ToAssignRoomToDoctorDto()).ToList();
        }

        public async Task<List<AssignWorksDto>> GetByDateAsync(DateOnly date)
        {
            var assignments = await _context.AssignWorks
                .Include(a => a.Room)
                .Include(a => a.user)
                .Include(a => a.Shift)
                .Where(a => a.StartDate <= date && (a.EndDate == null || a.EndDate >= date))
                .AsNoTracking()
                .ToListAsync();

            return assignments.Select(a => a.ToAssignRoomToDoctorDto()).ToList();
        }

        public async Task<List<AssignWorksDto>> GetByDateAndDoctorIdAsync(DateOnly date, string doctorId)
        {
            var assignments = await _context.AssignWorks
                .Include(a => a.Room)
                .Include(a => a.user)
                .Include(a => a.Shift)
                .Where(a => a.StartDate <= date && (a.EndDate == null || a.EndDate >= date) && a.userId == doctorId)
                .AsNoTracking()
                .ToListAsync();

            return assignments.Select(a => a.ToAssignRoomToDoctorDto()).ToList();
        }

        public async Task<bool> IsRoomAssignedToSameDoctorAsync(int roomId, string doctorId)
        {
            return await _context.AssignWorks
                .AnyAsync(a => a.RoomId == roomId && a.userId == doctorId);
        }

        public async Task<bool> IsRoomExistAsync(int roomId)
        {
            return await _context.Rooms.AnyAsync(r => r.Id == roomId);
        }

        public async Task<List<AssignWorksDto>> GetAssignWorksForAttendanceByDate(DateOnly date)
        {
            var assignments = await _context.AssignWorks
                .Include(a => a.Room)
                .Include(a => a.user)
                .Include(a => a.Shift)
                .Where(a => a.StartDate <= date && (a.EndDate == null || a.EndDate >= date))
                 .GroupBy(a => a.userId)
                .ToListAsync();

            return assignments.Select(g => g.First().ToAssignRoomToDoctorDto()).ToList();
        }
        public async Task<List<AssignWorksDto>> GetAssignWorksForAttendancetoday()
        {
            var assignments = await _context.AssignWorks
                .Include(a => a.Room)
                .Include(a => a.user)
                .Include(a => a.Shift)
                .Where(a => a.StartDate <= DateOnly.FromDateTime(DateTime.Now) && (a.EndDate == null || a.EndDate >= DateOnly.FromDateTime(DateTime.Now)))
                .AsNoTracking()
                .GroupBy(a => a.userId)
                .ToListAsync();

            return assignments.Select(g => g.First().ToAssignRoomToDoctorDto()).ToList();
        }
    }
}
