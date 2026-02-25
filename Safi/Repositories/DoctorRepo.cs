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
           var Doctor =await _context.Doctors.FirstOrDefaultAsync(d => d.Id == doctorId);
           if (Doctor == null)
           {
               return null;
           }
           var Reports = await _context.ReportDoctorToPatients.Include(p => p.Patient).Where(p => p.DoctorId == doctorId).ToListAsync();
           var Patients = Reports.Select(r =>r.Patient).ToList();
          return Patients.Select(p => new GetPatientsDto
          {
              Id = p.Id,
              Name = p.Name,
              Email = p.Email,
              Phone = p.PhoneNumber,
              Image=p.Image,
              History=p.History,
              HasSugar=p.HasSugar,
              HasPressure=p.HasPressure,
              Departments=p.Departments.Select(d => new DepartmentInfoDto
              {
                  Id = d.Id,
                  Name = d.Name,      
              }).ToList(),
          }).GroupBy(p => p.Id).Select(g => g.First()).ToList();
        }
        public async Task<List<GetPatientsDto>> GetallpatientsdealwithDoctorReservation(string doctorId)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Reservations)
                .FirstOrDefaultAsync(d => d.Id == doctorId);
            if (doctor?.Reservations == null)
                return null;
          var Reservations = await _context.Reservations
                .Include(r => r.Patient)
                .Where(r => r.DoctorId == doctorId)
                .ToListAsync();
           var patients = Reservations.Select(r => r.Patient).ToList();
           return patients.Select(p => p.ToGetPatientsDto()).GroupBy(p => p.Id).Select(g => g.First()).ToList();
        }
        public async Task<Doctor?> GetByIdAsync(string doctorId)
        {
            return await _context.Doctors
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
    }
}
