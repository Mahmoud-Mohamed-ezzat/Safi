using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Safi.Dto.Account;
using Safi.Dto.Department;
using Safi.Interfaces;
using Safi.Models;
using Safi.Dto.Reservation;
using Safi.Mapper;

namespace Safi.Repositories
{
    public class DoctorRepo : IDoctor
    {
        private readonly SafiContext _context;

        public DoctorRepo(SafiContext context)
        {
            _context = context;
        }

        public async Task<List<GetPatientsDto>> GetallpatientsdealwithDoctor(string doctorId)
        {
            var Doctor = await _context.Doctors.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.Id == doctorId);
            if (Doctor == null)
            {
                return null;
            }
            var Reports = await _context.ReportDoctorToPatients.IgnoreQueryFilters().Include(p => p.Patient).Where(p => p.DoctorId == doctorId).ToListAsync();
            var Patients = Reports.Select(r => r.Patient).ToList();
            return Patients.Select(p => new GetPatientsDto
            {
                Id = p.Id,
                Name = p.Name,
                Email = p.Email,
                Phone = p.PhoneNumber,
                Image = p.Image,
                History = p.History,
                HasSugar = p.HasSugar,
                HasPressure = p.HasPressure,
                IsDeleted = p.IsDeleted,
                IsActive = p.IsActive,
                Departments = p.Departments.Select(d => new DepartmentInfoDto
                {
                    Id = d.Id,
                    Name = d.Name,
                }).ToList(),
            }).GroupBy(p => p.Id).Select(g => g.First()).ToList();
        }
        public async Task<List<GetPatientsDto>> GetallpatientsdealwithDoctorReservation(string doctorId)
        {
            var doctor = await _context.Doctors.IgnoreQueryFilters()
                    .Include(d => d.Reservations)
                    .Include(d => d.Department)
                    .FirstOrDefaultAsync(d => d.Id == doctorId);
            if (doctor?.Reservations == null)
                return null;

            var Reservations = await _context.Reservations.IgnoreQueryFilters()
                .Include(r => r.Patient)
                .Where(r => r.DoctorId == doctorId)
                .ToListAsync();

            var patients = Reservations.Select(r => r.Patient).Where(p => p != null).ToList();
            return patients.Select(p =>
            {
                var dto = p.ToGetPatientsDto();
                dto.IsDeleted = p.IsDeleted;
                dto.IsActive = p.IsActive;
                return dto;
            }).GroupBy(p => p.Id).Select(g => g.First()).ToList();
        }
        public async Task<Doctor?> GetByIdAsync(string doctorId)
        {
            return await _context.Doctors.IgnoreQueryFilters()
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.Id == doctorId);
        }

        public async Task<bool> RateDoctorAsync(string doctorId, int rating)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == doctorId);
            if (doctor == null)
            {
                return false;
            }

            // Formula: NewAverage = ((OldRank * OldCount) + NewRating) / (OldCount + 1)
            float totalPoints = (doctor.Rank * doctor.RankCount) + rating;
            doctor.RankCount++;
            doctor.Rank = totalPoints / doctor.RankCount;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDoctorAsync(string doctorId)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            if (doctor == null) return false;

            doctor.IsDeleted = true;

            // 1. Delete future reservations
            var futureReservations = _context.Reservations.Where(r => r.DoctorId == doctorId && r.Time > DateTime.Now);
            _context.Reservations.RemoveRange(futureReservations);

            // 2. Close active room assignments
            var activeAssignments = _context.AssignWorks.Where(a => a.userId == doctorId && (a.EndDate == null || a.EndDate > DateOnly.FromDateTime(DateTime.Now)));
            foreach (var assignment in activeAssignments)
            {
                assignment.EndDate = DateOnly.FromDateTime(DateTime.Now);
            }

            // 3. Reassign active appointments
            var activeAppointments = await _context.AppointmentToRooms
                .Where(a => a.DoctorId == doctorId && a.EndTime == null)
                .ToListAsync();

            foreach (var appointment in activeAppointments)
            {
                // Try priority 1: Another doctor in the same room right now
                var anotherDoctorInRoom = await _context.AssignWorks
                    .Where(a => a.RoomId == appointment.RoomId && a.userId != doctorId && (a.EndDate == null || a.EndDate >= DateOnly.FromDateTime(DateTime.Now)))
                    .Select(a => a.userId)
                    .FirstOrDefaultAsync();

                if (anotherDoctorInRoom != null)
                {
                    appointment.DoctorId = anotherDoctorInRoom;
                }
                else
                {
                    // Try priority 2: Doctor in same shift and department
                    // Need to find which shift the current time falls into or which shift the doctor was assigned to
                    var currentShift = await _context.AssignWorks
                        .Where(a => a.userId == doctorId && a.RoomId == appointment.RoomId)
                        .Select(a => a.ShiftId)
                        .FirstOrDefaultAsync();

                    var fallbackDoctor = await _context.AssignWorks
                        .Include(a => a.user)
                        .Where(a => a.ShiftId == currentShift
                                    && a.userId != doctorId
                                    && a.user != null
                                    && doctor.DepartmentId != null
                                    && (a.user is Doctor) && ((Doctor)a.user).DepartmentId == doctor.DepartmentId
                                    && (a.EndDate == null || a.EndDate >= DateOnly.FromDateTime(DateTime.Now)))
                        .Select(a => a.userId)
                        .FirstOrDefaultAsync();

                    if (fallbackDoctor != null)
                    {
                        appointment.DoctorId = fallbackDoctor;
                    }
                    else
                    {
                        // Priority 3: Null
                        appointment.DoctorId = null;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
