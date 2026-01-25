using Microsoft.EntityFrameworkCore;
using Safi.Dto.EmergencyDto;
using Safi.Exceptions;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;

namespace Safi.Repositories
{
    public class EmergencyRepo : IEmergency
    {
        private readonly SafiContext _context;

        public EmergencyRepo(SafiContext context)
        {
            _context = context;
        }

        public async Task<List<EmergencyDto>> GetAllAsync()
        {
            var emergencies = await _context.Emergencies
                .Include(r => r.Department)
                .AsNoTracking()
                .ToListAsync();

            return emergencies.Select(r => r.ToEmergencyDto()).ToList();
        }

        public async Task<EmergencyDto?> GetByIdAsync(int id)
        {
            var emergency = await _context.Emergencies
                .Include(r => r.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            return emergency?.ToEmergencyDto();
        }

        public async Task<EmergencyDto> CreateAsync(CreateEmergencyDto dto)
        {
            try
            {
                if (!await IsRoomNumberUniqueAsync(dto.Number, dto.DepartmentId))
                {
                    throw new InvalidOperationException($"Emergency room number {dto.Number} already exists in department {dto.DepartmentId}");
                }
                var emergency = dto.ToEmergency();
                await _context.Emergencies.AddAsync(emergency);
                await _context.SaveChangesAsync();

                // Reload to get navigation properties
                await _context.Entry(emergency).Reference(r => r.Department).LoadAsync();
                return emergency.ToEmergencyDto();
            }
            catch (RoomNumberAlreadyExistsException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while creating emergency: {ex.Message}", ex);
            }
        }

        public async Task<EmergencyDto?> UpdateAsync(int id, UpdateEmergencyDto dto)
        {
            try
            {
                var emergency = await _context.Emergencies.FirstOrDefaultAsync(r => r.Id == id);
                if (emergency == null) return null;

                if (!await IsRoomNumberUniqueAsync(dto.Number, dto.DepartmentId, id))
                {
                    throw new InvalidOperationException($"Emergency room number {dto.Number} already exists in department {dto.DepartmentId}");
                }

                emergency.Number = dto.Number;
                emergency.DepartmentId = dto.DepartmentId;
                await _context.SaveChangesAsync();

                // Ensure navigation properties are available
                await _context.Entry(emergency).Reference(r => r.Department).LoadAsync();
                return emergency.ToEmergencyDto();
            }
            catch (RoomNumberAlreadyExistsException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while updating emergency: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var emergency = await _context.Emergencies.FirstOrDefaultAsync(r => r.Id == id);
            if (emergency == null) return false;

            _context.Emergencies.Remove(emergency);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<EmergencyDto>> GetByDepartmentIdAsync(int departmentId)
        {
            var emergencies = await _context.Emergencies
                .Include(r => r.Department)
                .Where(r => r.DepartmentId == departmentId)
                .AsNoTracking()
                .ToListAsync();

            return emergencies.Select(r => r.ToEmergencyDto()).ToList();
        }

        public async Task<List<EmergencyDto>> GetByDepartmentNameAsync(string departmentName)
        {
            var emergencies = await _context.Emergencies
                .Include(r => r.Department)
                .Where(r => r.Department != null
                         && r.Department.Name != null
                         && r.Department.Name.Contains(departmentName))
                .AsNoTracking()
                .ToListAsync();

            return emergencies.Select(r => r.ToEmergencyDto()).ToList();
        }

        public async Task<List<EmergencyDto>> GetByRoomNumberAsync(int roomNumber)
        {
            var emergencies = await _context.Emergencies
                .Include(r => r.Department)
                .Where(r => r.Number == roomNumber)
                .AsNoTracking()
                .ToListAsync();

            return emergencies.Select(r => r.ToEmergencyDto()).ToList();
        }

        public async Task<bool> IsRoomNumberUniqueAsync(int roomNumber, int departmentId, int? excludeId = null)
        {
            return !await _context.Emergencies
                .AnyAsync(r => r.Number == roomNumber
                            && r.DepartmentId == departmentId
                            && (!excludeId.HasValue || r.Id != excludeId.Value));
        }
    }
}
