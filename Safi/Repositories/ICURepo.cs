using Microsoft.EntityFrameworkCore;
using Safi.Dto.ICUDto;
using Safi.Exceptions;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;

namespace Safi.Repositories
{
    public class ICURepo : IICU
    {
        private readonly SafiContext _context;

        public ICURepo(SafiContext context)
        {
            _context = context;
        }

        public async Task<List<ICUDto>> GetAllAsync()
        {
            var icus = await _context.Icus
                .Include(r => r.Department)
                .AsNoTracking()
                .ToListAsync();

            return icus.Select(r => r.ToICUDto()).ToList();
        }

        public async Task<ICUDto?> GetByIdAsync(int id)
        {
            var icu = await _context.Icus
                .Include(r => r.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            return icu?.ToICUDto();
        }

        public async Task<ICUDto> CreateAsync(CreateICUDto dto)
        {
            try
            {
                if (!await IsRoomNumberUniqueAsync(dto.Number, dto.DepartmentId))
                {
                    throw new InvalidOperationException($"ICU room number {dto.Number} already exists in department {dto.DepartmentId}");
                }
                var icu = dto.ToICU();

                await _context.Icus.AddAsync(icu);
                await _context.SaveChangesAsync();

                // Reload to get navigation properties
                await _context.Entry(icu).Reference(r => r.Department).LoadAsync();
                return icu.ToICUDto();
            }
            catch (RoomNumberAlreadyExistsException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while creating ICU: {ex.Message}", ex);
            }
        }

        public async Task<ICUDto?> UpdateAsync(int id, UpdateICUDto dto)
        {
            try
            {
                var icu = await _context.Icus.FirstOrDefaultAsync(r => r.Id == id);
                if (icu == null) return null;

                if (!await IsRoomNumberUniqueAsync(dto.Number, dto.DepartmentId, id))
                {
                    throw new InvalidOperationException($"ICU room number {dto.Number} already exists in department {dto.DepartmentId}");
                }

                icu.Number = dto.Number;
                icu.DepartmentId = dto.DepartmentId;
                await _context.SaveChangesAsync();

                // Ensure navigation properties are available
                await _context.Entry(icu).Reference(r => r.Department).LoadAsync();
                return icu.ToICUDto();
            }
            catch (RoomNumberAlreadyExistsException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while updating ICU: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var icu = await _context.Icus.FirstOrDefaultAsync(r => r.Id == id);
            if (icu == null) return false;

            _context.Icus.Remove(icu);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ICUDto>> GetByDepartmentIdAsync(int departmentId)
        {
            var icus = await _context.Icus
                .Include(r => r.Department)
                .Where(r => r.DepartmentId == departmentId)
                .AsNoTracking()
                .ToListAsync();

            return icus.Select(r => r.ToICUDto()).ToList();
        }

        public async Task<List<ICUDto>> GetByDepartmentNameAsync(string departmentName)
        {
            var icus = await _context.Icus
                .Include(r => r.Department)
                .Where(r => r.Department != null
                         && r.Department.Name != null
                         && r.Department.Name.Contains(departmentName))
                .AsNoTracking()
                .ToListAsync();

            return icus.Select(r => r.ToICUDto()).ToList();
        }

        public async Task<List<ICUDto>> GetByRoomNumberAsync(int roomNumber)
        {
            var icus = await _context.Icus
                .Include(r => r.Department)
                .Where(r => r.Number == roomNumber)
                .AsNoTracking()
                .ToListAsync();

            return icus.Select(r => r.ToICUDto()).ToList();
        }

        public async Task<bool> IsRoomNumberUniqueAsync(int roomNumber, int departmentId, int? excludeId = null)
        {
            return !await _context.Icus
                .AnyAsync(r => r.Number == roomNumber
                            && r.DepartmentId == departmentId
                            && (!excludeId.HasValue || r.Id != excludeId.Value));
        }
    }
}
