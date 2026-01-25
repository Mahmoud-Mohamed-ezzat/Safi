using Microsoft.EntityFrameworkCore;
using Safi.Dto.RoomDto;
using Safi.Exceptions;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;

namespace Safi.Repositories
{
    public class RoomRepo : IRoom
    {
        private readonly SafiContext _context;

        public RoomRepo(SafiContext context)
        {
            _context = context;
        }

        public async Task<List<RoomDto>> GetAllAsync()
        {
            var rooms = await _context.Rooms
                .Include(r => r.Department)
                .Where(r => r.GetType() == typeof(Room))
                .AsNoTracking()
                .ToListAsync();

            return rooms.Select(r => r.ToRoomDto()).ToList();
        }

        public async Task<List<RoomDto>> GetAllRoomsByTypeAsync(string type)
        {
            var rooms = await _context.Rooms
                .Include(r => r.Department)
                .Where(r => r.GetType() == typeof(Room) && r.GetType().Name == type)
                .AsNoTracking()
                .ToListAsync();

            return rooms.Select(r => r.ToRoomDto()).ToList();
        }

        public async Task<RoomDto?> GetByIdAsync(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Department)
                .Where(r => r.GetType() == typeof(Room))
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            return room?.ToRoomDto();
        }

        public async Task<RoomDto> CreateAsync(CreateRoomDto dto)
        {
            try
            {
                if (!await IsRoomNumberUniqueAsync(dto.Number, dto.DepartmentId))
                {
                    throw new InvalidOperationException($"Room number {dto.Number} already exists in department {dto.DepartmentId}");
                }
                var room = dto.ToRoom();

                await _context.Rooms.AddAsync(room);
                await _context.SaveChangesAsync();

                // Reload to get navigation properties
                await _context.Entry(room).Reference(r => r.Department).LoadAsync();
                return room.ToRoomDto();
            }
            catch (RoomNumberAlreadyExistsException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while creating room: {ex.Message}", ex);
            }
        }

        public async Task<RoomDto?> UpdateAsync(int id, UpdateRoomDto dto)
        {
            try
            {
                var room = await _context.Rooms
                    .Where(r => r.GetType() == typeof(Room))
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (room == null) return null;

                if (!await IsRoomNumberUniqueAsync(dto.Number, dto.DepartmentId, id))
                {
                    throw new InvalidOperationException($"Room number {dto.Number} already exists in department {dto.DepartmentId}");
                }
                room.Number = dto.Number;
                room.DepartmentId = dto.DepartmentId;
                await _context.SaveChangesAsync();

                // Ensure navigation properties are available
                await _context.Entry(room).Reference(r => r.Department).LoadAsync();
                return room.ToRoomDto();
            }
            catch (RoomNumberAlreadyExistsException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while updating room: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var room = await _context.Rooms
                .Where(r => r.GetType() == typeof(Room))
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null) return false;

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<RoomDto>> GetByDepartmentIdAsync(int departmentId)
        {
            var rooms = await _context.Rooms
                .Include(r => r.Department)
                .Where(r => r.GetType() == typeof(Room) && r.DepartmentId == departmentId)
                .AsNoTracking()
                .ToListAsync();

            return rooms.Select(r => r.ToRoomDto()).ToList();
        }

        public async Task<List<RoomDto>> GetByDepartmentNameAsync(string departmentName)
        {
            var rooms = await _context.Rooms
                .Include(r => r.Department)
                .Where(r => r.GetType() == typeof(Room)
                         && r.Department != null
                         && r.Department.Name != null
                         && r.Department.Name.Contains(departmentName))
                .AsNoTracking()
                .ToListAsync();

            return rooms.Select(r => r.ToRoomDto()).ToList();
        }

        public async Task<List<RoomDto>> GetByRoomNumberAsync(int roomNumber)
        {
            var rooms = await _context.Rooms
                .Include(r => r.Department)
                .Where(r => r.GetType() == typeof(Room) && r.Number == roomNumber)
                .AsNoTracking()
                .ToListAsync();

            return rooms.Select(r => r.ToRoomDto()).ToList();
        }

        public async Task<bool> IsRoomNumberUniqueAsync(int roomNumber, int departmentId, int? excludeId = null)
        {
            return !await _context.Rooms
                .AnyAsync(r => r.GetType() == typeof(Room)
                            && r.Number == roomNumber
                            && r.DepartmentId == departmentId
                            && (!excludeId.HasValue || r.Id != excludeId.Value));
        }
    }
}
