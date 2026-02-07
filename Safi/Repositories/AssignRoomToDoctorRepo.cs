using Microsoft.EntityFrameworkCore;
using Safi.Dto.AssignRoomToDoctorDto;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;

namespace Safi.Repositories
{
    public class AssignRoomToDoctorRepo : IAssignRoomToDoctor
    {
        private readonly SafiContext _context;

        public AssignRoomToDoctorRepo(SafiContext context)
        {
            _context = context;
        }

        public async Task<List<AssignRoomToDoctorDto>> GetAllAsync()
        {
            var assignments = await _context.AssignRoomToDoctors
                .Include(a => a.Room)
                .Include(a => a.Doctor)
                .AsNoTracking()
                .ToListAsync();

            return assignments.Select(a => a.ToAssignRoomToDoctorDto()).ToList();
        }

        public async Task<AssignRoomToDoctorDto?> GetByIdAsync(int id)
        {
            var assignment = await _context.AssignRoomToDoctors
                .Include(a => a.Room)
                .Include(a => a.Doctor)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            return assignment?.ToAssignRoomToDoctorDto();
        }

        public async Task<AssignRoomToDoctorDto> CreateAsync(CreateAssignRoomToDoctorDto dto)
        {
            try
            {
                // Load doctor and room to validate department match
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == dto.DoctorId);
                var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == dto.RoomId);

                if (doctor == null)
                {
                    throw new InvalidOperationException($"Doctor with ID {dto.DoctorId} not found.");
                }

                if (room == null)
                {
                    throw new InvalidOperationException($"Room with ID {dto.RoomId} not found.");
                }

                // Validate that doctor's department matches room's department
                if (doctor.DepartmentId != room.DepartmentId)
                {
                    throw new InvalidOperationException($"Doctor {dto.DoctorId} belongs to department {doctor.DepartmentId}, but room {dto.RoomId} belongs to department {room.DepartmentId}. Doctors can only be assigned to rooms in their department.");
                }

                // Check if this room is already assigned to the same doctor
                var existingAssignment = await _context.AssignRoomToDoctors
                    .FirstOrDefaultAsync(a => a.RoomId == dto.RoomId && a.DoctorId == dto.DoctorId);

                if (existingAssignment != null)
                {
                    throw new InvalidOperationException($"Room {dto.RoomId} is already assigned to doctor {dto.DoctorId}.");
                }

                var assignment = dto.ToCreateAssignRoomToDoctorDto();
                await _context.AssignRoomToDoctors.AddAsync(assignment);
                await _context.SaveChangesAsync();

                // Load navigation properties
                await _context.Entry(assignment).Reference(a => a.Room).LoadAsync();
                await _context.Entry(assignment).Reference(a => a.Doctor).LoadAsync();

                return assignment.ToAssignRoomToDoctorDto();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Failed to create room assignment: {ex.Message}", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Database error occurred while creating room assignment.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An unexpected error occurred while creating room assignment.", ex);
            }
        }

        public async Task<AssignRoomToDoctorDto?> UpdateAsync(int id, UpdateAssignRoomToDoctorDto dto)
        {
            try
            {
                var assignment = await _context.AssignRoomToDoctors.FirstOrDefaultAsync(a => a.Id == id);
                if (assignment == null) return null;

                // Determine new values
                var newRoomId = dto.RoomId.HasValue ? dto.RoomId.Value : assignment.RoomId;
                var newDoctorId = !string.IsNullOrEmpty(dto.DoctorId) ? dto.DoctorId : assignment.DoctorId;

                // Check if values are actually CHANGING
                bool roomChanged = dto.RoomId.HasValue && dto.RoomId.Value != assignment.RoomId;
                bool doctorChanged = !string.IsNullOrEmpty(dto.DoctorId) && dto.DoctorId != assignment.DoctorId;

                // Validate department match when doctor or room is being changed
                if (roomChanged || doctorChanged)
                {
                    var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == newDoctorId);
                    var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == newRoomId);

                    if (doctor == null)
                    {
                        throw new InvalidOperationException($"Doctor with ID {newDoctorId} not found.");
                    }

                    if (room == null)
                    {
                        throw new InvalidOperationException($"Room with ID {newRoomId} not found.");
                    }

                    // Validate that doctor's department matches room's department
                    if (doctor.DepartmentId != room.DepartmentId)
                    {
                        throw new InvalidOperationException($"Doctor {newDoctorId} belongs to department {doctor.DepartmentId}, but room {newRoomId} belongs to department {room.DepartmentId}. Doctors can only be assigned to rooms in their department.");
                    }

                    // Check for duplicate room-doctor combination
                    var existingAssignment = await _context.AssignRoomToDoctors
                        .FirstOrDefaultAsync(a => a.RoomId == newRoomId &&
                                                  a.DoctorId == newDoctorId &&
                                                  a.Id != id);

                    if (existingAssignment != null)
                    {
                        throw new InvalidOperationException($"Room {newRoomId} is already assigned to doctor {newDoctorId}.");
                    }
                }

                // Partial Update Logic
                if (dto.RoomId.HasValue) assignment.RoomId = dto.RoomId.Value;
                if (!string.IsNullOrEmpty(dto.DoctorId)) assignment.DoctorId = dto.DoctorId;


                await _context.SaveChangesAsync();

                // Ensure navigation properties are available
                await _context.Entry(assignment).Reference(a => a.Room).LoadAsync();
                await _context.Entry(assignment).Reference(a => a.Doctor).LoadAsync();

                return assignment.ToAssignRoomToDoctorDto();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Failed to update room assignment: {ex.Message}", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Database error occurred while updating room assignment.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An unexpected error occurred while updating room assignment.", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var assignment = await _context.AssignRoomToDoctors.FirstOrDefaultAsync(a => a.Id == id);
            if (assignment == null) return false;
            assignment.EndDate = DateOnly.FromDateTime(DateTime.Now);
            _context.AssignRoomToDoctors.Remove(assignment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<AssignRoomToDoctorDto>> GetByDoctorIdAsync(string doctorId)
        {
            var assignments = await _context.AssignRoomToDoctors
                .Include(a => a.Room)
                .Include(a => a.Doctor)
                .Where(a => a.DoctorId == doctorId)
                .AsNoTracking()
                .ToListAsync();

            return assignments.Select(a => a.ToAssignRoomToDoctorDto()).ToList();
        }

        public async Task<List<AssignRoomToDoctorDto>> GetByRoomIdAsync(int roomId)
        {
            var assignments = await _context.AssignRoomToDoctors
                .Include(a => a.Room)
                .Include(a => a.Doctor)
                .Where(a => a.RoomId == roomId)
                .AsNoTracking()
                .ToListAsync();

            return assignments.Select(a => a.ToAssignRoomToDoctorDto()).ToList();
        }

        public async Task<List<AssignRoomToDoctorDto>> GetByDateAsync(DateOnly date)
        {
            var assignments = await _context.AssignRoomToDoctors
                .Include(a => a.Room)
                .Include(a => a.Doctor)
                .Where(a => a.StartDate <= date && a.EndDate >= date)
                .AsNoTracking()
                .ToListAsync();

            return assignments.Select(a => a.ToAssignRoomToDoctorDto()).ToList();
        }

        public async Task<List<AssignRoomToDoctorDto>> GetByDateAndDoctorIdAsync(DateOnly date, string doctorId)
        {
            var assignments = await _context.AssignRoomToDoctors
                .Include(a => a.Room)
                .Include(a => a.Doctor)
                .Where(a => a.StartDate <= date && a.EndDate >= date && a.DoctorId == doctorId)
                .AsNoTracking()
                .ToListAsync();

            return assignments.Select(a => a.ToAssignRoomToDoctorDto()).ToList();
        }


        public async Task<bool> IsRoomAssignedToSameDoctorAsync(int roomId, string doctorId)
        {
            return await _context.AssignRoomToDoctors
                .AnyAsync(a => a.RoomId == roomId && a.DoctorId == doctorId);
        }

        public async Task<bool> IsRoomExistAsync(int roomId)
        {
            return !await _context.AssignRoomToDoctors
                .AnyAsync(a => a.RoomId == roomId);
        }
    }
}
