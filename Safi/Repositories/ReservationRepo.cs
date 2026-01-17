using Microsoft.EntityFrameworkCore;
using Safi.Dto.Reservation;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;
using Microsoft.AspNetCore.SignalR;
using Safi.Hubs;
using System.ComponentModel.DataAnnotations;
namespace Safi.Repositories
{
    public class ReservationRepo : IReservation
    {
        private readonly SafiContext _context;

        private readonly IHubContext<ReservationHub> _hubContext;
        public ReservationRepo(SafiContext context, IHubContext<ReservationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        public async Task<Reservation> CreateOneReservation(CreateReservationDto dto)
        {
            var reservation = dto.ToCreateReservationDto();
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
            // Reload with Doctor navigation property
            var createdReservation = await _context.Reservations
                .Include(t => t.Doctor)
                .FirstOrDefaultAsync(t => t.Id == reservation.Id);

            return createdReservation;
        }
        public async Task<List<Reservation>> CreateManyReservations(CreatemanyReservationsDto dto)
        {
            var slotDuration = (dto.EndTime - dto.StartTime) / dto.slots;
            var reservations = new List<Reservation>(dto.slots);

            for (int i = 0; i < dto.slots; i++)
            {
                reservations.Add(new Reservation
                {
                    DoctorId = dto.DoctorId,
                    PatientId = dto.PatientId,
                    Time = dto.StartTime + (slotDuration * i),
                    Status = dto.Status
                });
            }
            await _context.Reservations.AddRangeAsync(reservations);
            await _context.SaveChangesAsync();

            // Get the IDs of created reservations and reload with Doctor
            var createdIds = reservations.Select(r => r.Id).ToList();
            var createdReservations = await _context.Reservations
                .Include(r => r.Doctor)
                .Where(r => createdIds.Contains(r.Id))
                .ToListAsync();

            return createdReservations;
        }
        public async Task<bool> DeleteManyReservationsByAvailableTimeId(int availableTimeId)
        {
            var availableTime = await _context.TimeAvailableOfDoctors
                   .FirstOrDefaultAsync(t => t.Id == availableTimeId);
            if (availableTime == null)
                return false;

            var targetDate = availableTime.Day.ToDateTime(TimeOnly.MinValue);

            var reservations = await _context.Reservations
                .Where(r => r.DoctorId == availableTime.DoctorId && r.Time.Date == targetDate)
                .ToListAsync();

            if (reservations.Count == 0)
                return false;

            _context.Reservations.RemoveRange(reservations);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateManyReservationsByAvailableTimeId(int availableTimeId, UpdateManyReservationsDto dto)
        {
            var availableTime = await _context.TimeAvailableOfDoctors
                   .FirstOrDefaultAsync(t => t.Id == availableTimeId);
            if (availableTime == null)
                return false;
            var targetDate = availableTime.Day.ToDateTime(TimeOnly.MinValue);
            var existingReservations = await _context.Reservations
                .Where(r => r.DoctorId == availableTime.DoctorId && r.Time.Date == targetDate.Date)
                .OrderBy(r => r.Time)
                .ToListAsync();
            var slotDuration = (dto.EndTime - dto.StartTime) / dto.slots;
            int existingCount = existingReservations.Count;
            int targetCount = dto.slots;
            int minCount = Math.Min(existingCount, targetCount);
            // Update existing reservations (EF Core tracks changes automatically)
            for (int i = 0; i < minCount; i++)
            {
                var reservation = existingReservations[i];
                reservation.Time = dto.StartTime + (slotDuration * i);
                reservation.PatientId = dto.PatientId;
                reservation.Status = dto.Status;
            }
            // Add new reservations if needed
            if (targetCount > existingCount)
            {
                var newReservations = new List<Reservation>(targetCount - existingCount);
                for (int i = existingCount; i < targetCount; i++)
                {
                    newReservations.Add(new Reservation
                    {
                        DoctorId = dto.DoctorId,
                        PatientId = dto.PatientId,
                        Time = dto.StartTime + (slotDuration * i),
                        Status = dto.Status
                    });
                }
                _context.Reservations.AddRange(newReservations);
            }
            // Remove excess reservations if needed
            if (existingCount > targetCount)
            {
                _context.Reservations.RemoveRange(existingReservations.GetRange(targetCount, existingCount - targetCount));
            }
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Assignreservationforpatient(AssignReservationForpatient dto)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == dto.ReservationId);

            if (reservation == null)
                return false;

            // Check if patient already has a reservation with this doctor on this day  
            var existingReservation = await _context.Reservations
                .AnyAsync(r => r.DoctorId == reservation.DoctorId
                            && r.PatientId == dto.PatientID
                            && r.Time.Date == reservation.Time.Date
                            && r.Status == "Reserved");

            if (existingReservation)
            {
                throw new InvalidOperationException("You already have a reservation with this doctor on this day.");
            }

            reservation.PatientId = dto.PatientID;
            reservation.Status = "Reserved";
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {
                // Concurrency or DB error
                throw;
            }
            var dayStr = reservation.Time.ToString("yyyy-MM-dd");
            await _hubContext.Clients
             .Group($"{reservation.DoctorId}_{dayStr}")
             .SendAsync("SlotReserved", new
             {
                 ReservationId = reservation.Id,
                 Message = "This slot has been reserved by another patient",
                 DoctorId = reservation.DoctorId,
                 Day = dayStr,
                 Time = reservation.Time
             });
            return true;
        }
        public async Task<List<reservationInfoforgetpatientReservations>> GetReservationByPatientId(string patientId)
        {
            var resrvations = await _context.Reservations.AsNoTracking()
                .Where(r => r.PatientId == patientId)
                .ToListAsync();

            var patient = await _context.Patients.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == patientId);

            var reservationsDto = resrvations.Select(r => new reservationInfoforgetpatientReservations
            {
                PatientId = r.PatientId,
                PatientName = patient?.Name,
                DoctorId = r.DoctorId,
                StartTime = r.Time,
                EndTime = r.Time,
                Status = r.Status
            }).ToList();

            return reservationsDto;
        }

        public Task<List<reservationInfoforgetpatientReservations>> GetReservationByDoctorId(string doctorId)
        {
            throw new NotImplementedException();
        }

        public Task<List<reservationInfoforgetpatientReservations>> GetReservationByPatientIdAndDoctorId(string patientId, string doctorId)
        {
            throw new NotImplementedException();
        }

        public Task<List<reservationInfoforgetpatientReservations>> GetReservationByPatientIdAndDate(string patientId, DateTime date)
        {
            throw new NotImplementedException();
        }

        public Task<List<reservationInfoforgetpatientReservations>> GetReservationByDoctorIdAndDate(string doctorId, DateTime date)
        {
            throw new NotImplementedException();
        }

        public Task<List<reservationInfoforgetpatientReservations>> GetReservationByDate(DateTime date)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> addDepartmenttoPatientDepartmentWhenReservationIsCreated(addDepartmenttoPatientDepartmentWhenReservationIsCreatedDto dto)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.Id == dto.DoctorId);

            if (doctor?.Department == null)
                return null;

            var patient = await _context.Patients
                .Include(p => p.Departments)
                .FirstOrDefaultAsync(p => p.Id == dto.PatientId);

            if (patient == null)
                return null;

            // Check if department already exists in patient's list
            if (patient.Departments != null && !patient.Departments.Any(d => d.Id == doctor.Department.Id))
            {
                patient.Departments.Add(doctor.Department);
                await _context.SaveChangesAsync();
            }
            return doctor.Department.Name;
        }

    }
}
