using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Safi.Dto.AvailableTimeOFDoctor;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;
using Safi.Hubs;
namespace Safi.Repositories
{
    public class AvailableTimeOfDoctorRepo : IAvailableTimeOfDoctor
    {
        private readonly SafiContext _context;
        private readonly IReservation _reservation;
        private readonly IHubContext<ReservationHub> _hubContext;
        public AvailableTimeOfDoctorRepo(SafiContext context, IReservation reservation, IHubContext<ReservationHub> hubContext)
        {
            _context = context;
            _reservation = reservation;
            _hubContext = hubContext;
        }

        public async Task<List<AvailableTimeInfoDto>> GetAvailableTimesByDate(DateOnly day)
        {

            var entities = await _context.TimeAvailableOfDoctors
                            .Include(t => t.Doctor)
                            .AsNoTracking()
                            .Where(t => t.Day == day)
                            .ToListAsync();

            return entities.Select(e => e.ToAvailableTimeInfoDto()).ToList();


        }

        public async Task<AvailableTimeInfoDto> CreateAvailableTime(CreateAvailableTimeDto dto)
        {
            var entity = dto.AvailableTimeOFDoctorDto();
            var reservation = await _reservation.CreateManyReservations(dto.CreateManyReservationsFromDoctorTimesDto());
            _context.TimeAvailableOfDoctors.Add(entity);
            await _context.SaveChangesAsync();

            // Reload with Doctor navigation property
            var createdEntity = await _context.TimeAvailableOfDoctors
                .Include(t => t.Doctor)
                .FirstOrDefaultAsync(t => t.Id == entity.Id);

            var dayStr = entity.Day.ToString("yyyy-MM-dd");
            await _hubContext.Clients
                .Group($"{entity.DoctorId}_{dayStr}")
                .SendAsync("AvailabilityUpdated", new
                {
                    Message = "New appointment slots are now available",
                    DoctorId = entity.DoctorId,
                    Day = dayStr
                });
            return createdEntity!.ToAvailableTimeInfoDto();
        }

        public async Task<AvailableTimeInfoDto?> UpdateAvailableTimebyreceptionist(int id, UpdateAvailableTimeDto dto)
        {
            var entity = await _context.TimeAvailableOfDoctors
                .Include(t => t.Doctor)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (entity == null)
                return null;

            entity.DoctorId = dto.DoctorId;
            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;
            entity.Day = dto.Day;
            entity.Slots = dto.Slots;
            await _reservation.UpdateManyReservationsByAvailableTimeId(id, dto.ToUpdateManyReservationsDto());
            await _context.SaveChangesAsync();

            // Reload to get updated Doctor if DoctorId changed
            var updatedEntity = await _context.TimeAvailableOfDoctors
                .Include(t => t.Doctor)
                .FirstOrDefaultAsync(t => t.Id == id);

            return updatedEntity!.ToAvailableTimeInfoDto();
        }
        public async Task<AvailableTimeInfoDto?> UpdateAvailableTimebyDoctor(int id, UpdateAvailableTimeDto2 dto)
        {
            var entity = await _context.TimeAvailableOfDoctors
                .Include(t => t.Doctor)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (entity == null)
                return null;

            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;
            entity.Day = dto.Day;
            entity.Slots = dto.Slots;
            await _reservation.UpdateManyReservationsByAvailableTimeId(id, dto.ToUpdateManyReservationsDto2(entity.DoctorId));
            await _context.SaveChangesAsync();
            var dayStr = entity.Day.ToString("yyyy-MM-dd");
            await _hubContext.Clients
                .Group($"{entity.DoctorId}_{dayStr}")
                .SendAsync("AvailabilityUpdated", new
                {
                    Message = "New appointment slots are now available",
                    DoctorId = entity.DoctorId,
                    Day = dayStr
                });
            // Reload to get updated Doctor if DoctorId changed
            var updatedEntity = await _context.TimeAvailableOfDoctors
                .Include(t => t.Doctor)
                .FirstOrDefaultAsync(t => t.Id == id);

            return updatedEntity!.ToAvailableTimeInfoDto();
        }



        public async Task<AvailableTimeInfoDto?> GetAvailableTimeById(int id)
        {
            var entity = await _context.TimeAvailableOfDoctors
                .Include(t => t.Doctor)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            return entity?.ToAvailableTimeInfoDto();
        }


        public async Task<List<AvailableTimeInfoDto>> GetAllAvailableTimes()
        {
            var entities = await _context.TimeAvailableOfDoctors
                .Include(t => t.Doctor)
                .AsNoTracking()
                .ToListAsync();

            return entities.Select(e => e.ToAvailableTimeInfoDto()).ToList();
        }

        public async Task<List<AvailableTimeInfoDto>> GetAvailableTimesByDoctorId(string doctorId)
        {
            var entities = await _context.TimeAvailableOfDoctors
                .Include(t => t.Doctor)
                .AsNoTracking()
                .Where(t => t.DoctorId == doctorId)
                .ToListAsync();

            return entities.Select(e => e.ToAvailableTimeInfoDto()).ToList();
        }

        public async Task<List<AvailableTimeInfoDto>> GetAvailableTimesByDoctorName(string doctorName)
        {
            var entities = await _context.TimeAvailableOfDoctors
                .Include(t => t.Doctor)
                .AsNoTracking()
                .Where(t => t.Doctor.Name == doctorName)
                .ToListAsync();

            return entities.Select(e => e.ToAvailableTimeInfoDto()).ToList();
        }
        public async Task<bool> DeleteAvailableTime(int id)
        {
            var entity = await _context.TimeAvailableOfDoctors.FirstOrDefaultAsync(t => t.Id == id);

            if (entity == null)
                return false;

            _context.TimeAvailableOfDoctors.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<AvailableTimeInfoDto>> GetAvailableTimesByDepartment(string Department)
        {

            var entities = await _context.TimeAvailableOfDoctors
                .Include(t => t.Doctor)
                .AsNoTracking()
                .Where(t => t.Doctor.Department.Name.ToLower() == Department.ToLower())
                .ToListAsync();

            return entities.Select(e => e.ToAvailableTimeInfoDto()).ToList();

        }
        public async Task<List<AvailableTimeInfoDto>> GetAvailableTimesByDoctorIdAndDate(string doctorId, DateOnly day)
        {
            var entities = await _context.TimeAvailableOfDoctors
                .Include(t => t.Doctor)
                .AsNoTracking()
                .Where(t => t.DoctorId == doctorId && t.Day == day)
                .ToListAsync();

            return entities.Select(e => e.ToAvailableTimeInfoDto()).ToList();
        }
        public async Task<List<AvailableTimeInfoDto>> GetAvailableTimesOfDoctorByDateandTime(string doctorId, DateOnly day, TimeOnly time)
        {
            var entities = await _context.TimeAvailableOfDoctors
                .Include(t => t.Doctor)
                .AsNoTracking()
                .Where(t => t.DoctorId == doctorId && t.Day == day && t.StartTime.Hour == time.Hour && t.StartTime.Minute == time.Minute)
                .ToListAsync();

            return entities.Select(e => e.ToAvailableTimeInfoDto()).ToList();
        }
        public async Task<List<AvailableTimeInfoDto>> GetAllAvailableTimesByDateandTime(DateOnly day, TimeOnly time)
        {
            var entities = await _context.TimeAvailableOfDoctors
                .Include(t => t.Doctor)
                .AsNoTracking()
                .Where(t => t.Day == day && t.StartTime.Hour == time.Hour && t.StartTime.Minute == time.Minute)
                .ToListAsync();

            return entities.Select(e => e.ToAvailableTimeInfoDto()).ToList();
        }
    }
}
