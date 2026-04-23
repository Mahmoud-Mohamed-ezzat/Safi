using Microsoft.EntityFrameworkCore;
using Safi.Dto.Account;
using Safi.Dto.AppointmentToRoom;
using Safi.Dto.ReportDoctorToPatientDto;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;

namespace Safi.Repositories
{
    public class AppointmentToRoomRepo : IAppointmentToRoom
    {
        private readonly SafiContext _context;
        private readonly IReservation _reservation;

        public AppointmentToRoomRepo(SafiContext context, IReservation reservation)
        {
            _context = context;
            _reservation = reservation;
        }

        // Helper method to get query with all navigation properties included
        private IQueryable<AppointmentToRoom> GetQueryWithIncludes(bool ignoreFilters = false)
        {
            var query = _context.AppointmentToRooms.AsQueryable();
            if (ignoreFilters) query = query.IgnoreQueryFilters();

            return query
                .Include(a => a.CreatedByUser)
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Room)
                    .ThenInclude(r => r!.Department);
        }

        // Helper method to safely convert nullable DateTime to DateOnly, defaulting to current date
        private static DateOnly GetEndDateOrNow(DateTime? endTime)
        {
            return DateOnly.FromDateTime(endTime ?? DateTime.UtcNow);
        }

        public async Task<AppointmentToRoomDto> CreateAsync(CreateAppointmentToRoomDto dto)
        {
            try
            {
                var appointment = dto.ToAppointmentToRoom();
                var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == dto.RoomId);


                await _context.AppointmentToRooms.AddAsync(appointment);
                await _reservation.addDepartmenttoPatientDepartmentWhenReservationIsCreated(dto.PatientId, dto.PrimaryDoctorId);

                // Also track Room's department if it differs or to be safe
                if (room?.DepartmentId != null)
                {
                    var patient = await _context.Patients.Include(p => p.Departments).FirstOrDefaultAsync(p => p.Id == dto.PatientId);
                    if (patient != null && !patient.Departments!.Any(d => d.Id == room.DepartmentId))
                    {
                        var dept = await _context.Departments.FindAsync(room.DepartmentId);
                        if (dept != null) patient.Departments.Add(dept);
                    }
                }

                await _context.SaveChangesAsync();

                // Load navigation properties
                await _context.Entry(appointment).Reference(a => a.CreatedByUser).LoadAsync();
                await _context.Entry(appointment).Reference(a => a.Patient).LoadAsync();
                await _context.Entry(appointment).Reference(a => a.Room).LoadAsync();
                if (appointment.Room != null)
                {
                    await _context.Entry(appointment.Room).Reference(r => r.Department).LoadAsync();
                }

                return appointment.ToAppointmentToRoomDto();
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? "No inner exception";
                throw new InvalidOperationException($"Error creating appointment: {ex.Message} | Inner: {innerMessage}", ex);
            }
        }

        public async Task<AppointmentToRoomDto?> GetByIdAsync(int id)
        {
            var appointment = await GetQueryWithIncludes(true)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            return appointment?.ToAppointmentToRoomDto();
        }

        public async Task<List<AppointmentToRoomDto>> GetByRoomIdAsync(int roomId)
        {
            var appointments = await GetQueryWithIncludes()
                .Where(a => a.RoomId == roomId)
                .AsNoTracking()
                .ToListAsync();

            return appointments.Select(a => a.ToAppointmentToRoomDto()).ToList();
        }

        public async Task<List<AppointmentToRoomDto>> GetByPatientIdAsync(string patientId)
        {
            var appointments = await GetQueryWithIncludes()
                .Where(a => a.PatientId == patientId)
                .AsNoTracking()
                .ToListAsync();

            return appointments.Select(a => a.ToAppointmentToRoomDto()).ToList();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var appointment = await _context.AppointmentToRooms.FirstOrDefaultAsync(a => a.Id == id);
            if (appointment == null) return false;

            _context.AppointmentToRooms.Remove(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AppointmentToRoomDto?> UpdateEndTimeAsync(CreateReportWhenPatientGetOutRoomDto dto)
        {
            var appointment = await _context.AppointmentToRooms.FirstOrDefaultAsync(a => a.Id == dto.id);
            if (appointment == null) return null;

            appointment.EndTime = dto.EndTime;
            var user = await _context.Patients.FirstOrDefaultAsync(u => u.Id == appointment.PatientId);

            if (user != null)
            {
                var doctorName = appointment.Doctor?.Name ?? "Unknown";
                var departmentName = appointment.Room?.Department?.Name ?? "Unknown";
                var medicines = dto.Medicines != null ? string.Join(", ", dto.Medicines) : "None";

                user.History += $" \n Appointment in room {appointment.RoomId} with doctor {doctorName} in department {departmentName} Report: {dto.Report} Medicines: {medicines} started at {appointment.StartTime} ended at {dto.EndTime}";
            }

            await _context.SaveChangesAsync();

            // Load navigation properties
            await _context.Entry(appointment).Reference(a => a.CreatedByUser).LoadAsync();
            await _context.Entry(appointment).Reference(a => a.Patient).LoadAsync();
            await _context.Entry(appointment).Reference(a => a.Room).LoadAsync();
            if (appointment.Room != null)
            {
                await _context.Entry(appointment.Room).Reference(r => r.Department).LoadAsync();
            }

            return appointment.ToAppointmentToRoomDto();
        }

        public async Task<AppointmentToRoomDto?> GetActiveAppointmentByRoomIdAsync(int roomId)
        {
            var appointment = await GetQueryWithIncludes()
                .Where(a => a.RoomId == roomId && a.EndTime == null)
                .OrderByDescending(a => a.StartTime)
                .FirstOrDefaultAsync();
            return appointment?.ToAppointmentToRoomDto();
        }
        public async Task<AppointmentToRoomDto?> GetActiveAppointmentByRoomIdAsync(int roomId,string doctorId)
        {
            var appointment = await GetQueryWithIncludes()
                .Where(a => a.RoomId == roomId && a.EndTime == null && a.DoctorId == doctorId||a.CreatedBy==doctorId)
                .OrderByDescending(a => a.StartTime)
                .FirstOrDefaultAsync();
            return appointment?.ToAppointmentToRoomDto();
        }
        public async Task<List<AppointmentToRoomDto>> GetAllAsync()
        {
            var appointments = await GetQueryWithIncludes(true)
                .AsNoTracking()
                .ToListAsync();
            return appointments.Select(a => a.ToAppointmentToRoomDto()).ToList();
        }
        public async Task<List<AppointmentToRoomDto>> GetByPatientIdandDateAsync(string patientId, DateOnly? Date)
        {
            var appointments = await _context.AppointmentToRooms
                .IgnoreQueryFilters()
                .Include(a => a.CreatedByUser)
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Room)
                    .ThenInclude(r => r!.Department)
                .Where(a => a.PatientId == patientId && a.StartTime.HasValue && DateOnly.FromDateTime(a.StartTime.Value) <= Date && DateOnly.FromDateTime(a.EndTime == null ? DateTime.UtcNow : a.EndTime.Value) >= Date)
                .AsNoTracking()
                .ToListAsync();

            return appointments.Select(a => a.ToAppointmentToRoomDto()).ToList();
        }

        public async Task<List<AppointmentToRoomDto>> GetByPrimaryDoctorIdAsync(string doctorId)
        {
            var appointments = await GetQueryWithIncludes(true)
                .Where(a => a.DoctorId == doctorId)
                .AsNoTracking()
                .ToListAsync();

            return appointments.Select(a => a.ToAppointmentToRoomDto()).ToList();
        }

        public async Task<List<AppointmentToRoomDto>> GetByPatientNameAsync(string PatientName)
        {
            var appointments = await GetQueryWithIncludes(true)
                .Where(a => a.Patient != null && a.Patient.Name != null && a.Patient.Name.ToLower() == PatientName.ToLower())
                .AsNoTracking()
                .ToListAsync();

            return appointments.Select(a => a.ToAppointmentToRoomDto()).ToList();
        }

        public async Task<List<AppointmentToRoomDto>> GetByDoctorNameAsync(string DoctorName)
        {
            var appointments = await GetQueryWithIncludes(true)
                .Where(a => a.Doctor != null && a.Doctor.Name != null && a.Doctor.Name.ToLower() == DoctorName.ToLower())
                .AsNoTracking()
                .ToListAsync();

            return appointments.Select(a => a.ToAppointmentToRoomDto()).ToList();
        }

        public async Task<List<AppointmentToRoomDto>> GetByDoctorIdandDateAsync(string doctorId, DateOnly? Date)
        {
            var appointments = await GetQueryWithIncludes(true)
                .Where(a => a.DoctorId == doctorId &&
                           a.StartTime.HasValue &&
                           DateOnly.FromDateTime(a.StartTime.Value) <= Date &&
                           DateOnly.FromDateTime(a.EndTime ?? DateTime.UtcNow) >= Date)
                .AsNoTracking()
                .ToListAsync();

            return appointments.Select(a => a.ToAppointmentToRoomDto()).ToList();
        }

        public async Task<List<AppointmentToRoomDto>> GetByRoomIdandDateAsync(int roomId, DateOnly? Date)
        {
            var appointments = await GetQueryWithIncludes(true)
                .Where(a => a.RoomId == roomId &&
                           a.StartTime.HasValue &&
                           DateOnly.FromDateTime(a.StartTime.Value) <= Date &&
                           DateOnly.FromDateTime(a.EndTime ?? DateTime.UtcNow) >= Date)
                .AsNoTracking()
                .ToListAsync();

            return appointments.Select(a => a.ToAppointmentToRoomDto()).ToList();
        }

        public async Task<List<AppointmentToRoomDto>> GetByPatientNameandDateAsync(string PatientName, DateOnly? Date)
        {
            var appointments = await GetQueryWithIncludes(true)
                .Where(a => a.Patient != null && a.Patient.Name != null &&
                           a.Patient.Name.ToLower() == PatientName.ToLower() &&
                           a.StartTime.HasValue &&
                           DateOnly.FromDateTime(a.StartTime.Value) <= Date &&
                           DateOnly.FromDateTime(a.EndTime ?? DateTime.UtcNow) >= Date)
                .AsNoTracking()
                .ToListAsync();

            return appointments.Select(a => a.ToAppointmentToRoomDto()).ToList();
        }

        public async Task<List<AppointmentToRoomDto>> GetByDoctorNameandDateAsync(string DoctorName, DateOnly? Date)
        {
            var appointments = await GetQueryWithIncludes(true)
                .Where(a => a.Doctor != null && a.Doctor.Name != null &&
                           a.Doctor.Name.ToLower() == DoctorName.ToLower() &&
                           a.StartTime.HasValue &&
                           DateOnly.FromDateTime(a.StartTime.Value) <= Date &&
                           DateOnly.FromDateTime(a.EndTime ?? DateTime.UtcNow) >= Date)
                .AsNoTracking()
                .ToListAsync();

            return appointments.Select(a => a.ToAppointmentToRoomDto()).ToList();
        }

        public async Task<List<AppointmentToRoomDto>> GetByPrimaryDoctorIdandDateAsync(string doctorId, DateOnly? Date)
        {
            var appointments = await GetQueryWithIncludes(true)
                .Where(a => a.DoctorId == doctorId &&
                           a.StartTime.HasValue &&
                           DateOnly.FromDateTime(a.StartTime.Value) <= Date &&
                           DateOnly.FromDateTime(a.EndTime ?? DateTime.UtcNow) >= Date)
                .AsNoTracking()
                .ToListAsync();

            return appointments.Select(a => a.ToAppointmentToRoomDto()).ToList();
        }

        public async Task<List<GetDoctorsDto>> GetAllDoctorsinthisRoomInthisAppointment(int AppointmentId)
        {
            var appointment = await _context.AppointmentToRooms
                .Include(a => a.Doctor)
                .Include(a => a.CreatedByUser)
                .Include(a => a.Patient)
                .Include(a => a.Room)
                  .ThenInclude(r => r.AssignRoomToDoctors)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == AppointmentId);

            // Return empty list if appointment not found or missing required data
            if (appointment == null || !appointment.RoomId.HasValue || !appointment.StartTime.HasValue)
            {
                return new List<GetDoctorsDto>();
            }

            var assigns = await _context.AssignRoomToDoctors.AsNoTracking()
            .Include(ad => ad.Doctor)
               .ThenInclude(d => d.Department)
            .Include(a => a.Room)
            .Where(ad => ad.RoomId == appointment.RoomId.Value &&
            ad.StartDate <= GetEndDateOrNow(appointment.EndTime) &&
            appointment.StartTime.HasValue &&
            ad.EndDate >= DateOnly.FromDateTime(appointment.StartTime.Value))
            .ToListAsync();
            var doctors = assigns.Select(ad => ad.Doctor).Where(d => d != null).ToList();
            return doctors.Select(d => d.ToGetDoctorsDto()).ToList();
        }

        public async Task<List<AppointmentToRoomDto>> GetAllAppointmentofDoctorasprimaryorNot(string DoctorId)
        {
            var appointments = await _context.AppointmentToRooms
                .IgnoreQueryFilters()
                .Include(a => a.CreatedByUser)
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Room)
                    .ThenInclude(r => r!.Department)
                .Where(a => a.DoctorId == DoctorId ||
                            _context.AssignRoomToDoctors.IgnoreQueryFilters().Any(ad =>
                                ad.DoctorId == DoctorId &&
                                ad.RoomId == a.RoomId &&
                                a.StartTime.HasValue &&
                                ad.EndDate >= DateOnly.FromDateTime(a.StartTime.Value) &&
                                (!a.EndTime.HasValue || ad.StartDate <= DateOnly.FromDateTime(a.EndTime == null ? DateTime.UtcNow : a.EndTime.Value))))
                .AsNoTracking()
                .ToListAsync();

            return appointments.Select(a => a.ToAppointmentToRoomDto()).ToList();
        }

        public async Task<List<GetPatientsDto>> GetallpatientsdealwithDoctor(string doctorId)
        {
            var Doctor = await _context.Doctors.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.Id == doctorId);
            if (Doctor == null)
                return null;

            var appointments = await _context.AppointmentToRooms
                .IgnoreQueryFilters()
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Room)
                    .ThenInclude(r => r!.Department)
                .Where(a => a.DoctorId == doctorId)
                .AsNoTracking()
                .ToListAsync();

            return appointments.Select(a => a.Patient).Select(p => p.ToGetPatientsDto()).GroupBy(p => p.Id).Select(g => g.First()).ToList();

        }
    }
}
