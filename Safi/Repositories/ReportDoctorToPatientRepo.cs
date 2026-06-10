using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Safi.Dto.ReportDoctorToPatientDto;
using Safi.Dto.Account;
using Safi.Helpers;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;

namespace Safi.Repositories
{
    public class ReportDoctorToPatientRepo : IReportDoctorToPatient
    {
        private readonly SafiContext _context;
        private readonly UserManager<User> _usermanager;
        private readonly IReservation _reservation;

        public ReportDoctorToPatientRepo(SafiContext context, UserManager<User> userManager, IReservation reservation)
        {
            _context = context;
            _usermanager = userManager;
            _reservation = reservation;
        }

        // Helper method to get query with all navigation properties included
        private IQueryable<ReportDoctorToPatient> GetQueryWithIncludes()
        {
            return _context.ReportDoctorToPatients
                .IgnoreQueryFilters()
                .Include(r => r.Patient)
                .Include(r => r.Doctor);
        }

        // Helper method to convert DateTime to DateOnly
        private static DateOnly ToDateOnly(DateTime dateTime)
        {
            return DateOnly.FromDateTime(dateTime);
        }

        public async Task<List<ReportDoctorToPatient>> GetAllAsync()
        {
            return await GetQueryWithIncludes()
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<ReportDoctorToPatient>> GetByDateAsyncandNameOfDoctor(DateOnly date, string doctorName)
        {
            return await GetQueryWithIncludes()
                .Where(r => r.Doctor != null && r.Doctor.Name != null &&
                           ToDateOnly(r.CreatedAt) == date && r.Doctor.Name.Contains(doctorName))
                .ToListAsync();
        }
        public async Task<ReportDoctorToPatient?> GetByIdAsync(int id)
        {
            return await GetQueryWithIncludes()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        public async Task<List<ReportDoctorToPatient>> GetByDateAsync(DateOnly date)
        {
            return await GetQueryWithIncludes()
                .Where(r => ToDateOnly(r.CreatedAt) == date)
        .ToListAsync();
        }
        public async Task<ReportDoctorToPatient> CreateAsync(CreateReportDoctorToPatientDto dto)
        {
            var report = dto.ToReportDoctorToPatient();
            report.CreatedAt = EgyptTime.Now;
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == dto.PatientId);
            var Doctor = await _context.Doctors.FirstOrDefaultAsync(p => p.Id == dto.DoctorId);
            if (patient != null && Doctor != null)
            {
                string medicinesList = string.Join(", ", dto.Medicines ?? new List<string>());
                patient.History += Environment.NewLine + $" Doctor:{Doctor.Name} make Report created At {report.CreatedAt} which contain {dto.Report} and medicines: {medicinesList}  ";
            }
            await _context.ReportDoctorToPatients.AddAsync(report);
            await _reservation.addDepartmenttoPatientDepartmentWhenReservationIsCreated(dto.PatientId, dto.DoctorId);
            await _context.SaveChangesAsync();

            // Reload to get navigation properties
            await _context.Entry(report).Reference(r => r.Patient).LoadAsync();
            await _context.Entry(report).Reference(r => r.Doctor).LoadAsync();

            return report;
        }

        public async Task<ReportDoctorToPatient?> UpdateAsync(int id, UpdateReportDoctorToPatientDto dto)
        {
            var report = await _context.ReportDoctorToPatients.FindAsync(id);
            if (report == null) return null;

            report.Report = dto.Report;
            report.Medicines = dto.Medicines;
            await _context.SaveChangesAsync();

            // Ensure navigation properties are available
            await _context.Entry(report).Reference(r => r.Patient).LoadAsync();
            await _context.Entry(report).Reference(r => r.Doctor).LoadAsync();

            return report;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var report = await _context.ReportDoctorToPatients.FindAsync(id);
            if (report == null) return false;

            _context.ReportDoctorToPatients.Remove(report);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<ReportDoctorToPatient>> GetByDateAsyncandNameOfPatient(DateOnly date, string patientName)
        {
            return await GetQueryWithIncludes()
                .Where(r => r.Patient != null && r.Patient.Name != null &&
                           ToDateOnly(r.CreatedAt) == date && r.Patient.Name.Contains(patientName))
                .ToListAsync();
        }

        public async Task<List<ReportDoctorToPatient>> GetByPatientIdAsync(string patientId)
        {
            return await GetQueryWithIncludes()
                .Where(r => r.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<List<ReportDoctorToPatient>> GetByDoctorIdAsync(string doctorId)
        {
            return await GetQueryWithIncludes()
                .Where(r => r.DoctorId == doctorId)
                .ToListAsync();
        }
        public async Task<List<ReportDoctorToPatient>> GetByDoctorIdAndDateAsync(string doctorId, DateOnly date)
        {
            return await GetQueryWithIncludes()
                .Where(r => r.DoctorId == doctorId && ToDateOnly(r.CreatedAt) == date)
                .ToListAsync();
        }
        public async Task<List<ReportDoctorToPatient>> GetByMedicineAndPatientAsync(string medicine, string patientId)
        {
            // Optimized to filter in the database using EF Core's primitive collection support
            return await GetQueryWithIncludes()
                .Where(r => r.PatientId == patientId && r.Medicines != null && r.Medicines.Contains(medicine))
                .ToListAsync();
        }
        public async Task<List<ReportDoctorToPatient>> GetByPatientNameAsync(string patientName)
        {
            return await GetQueryWithIncludes()
                .Where(r => r.Patient != null && r.Patient.Name != null && r.Patient.Name.Contains(patientName))
                .ToListAsync();
        }
        public async Task<List<ReportDoctorToPatient>> GetByDoctorNameAsync(string doctorName)
        {
            return await GetQueryWithIncludes()
                .Where(r => r.Doctor != null && r.Doctor.Name != null && r.Doctor.Name.Contains(doctorName))
                .ToListAsync();
        }
        public async Task<List<ReportDoctorToPatient>> GetByDoctorNameandPatientNameAsync(string doctorName, string patientName)
        {
            return await GetQueryWithIncludes()
               .Where(r => r.Doctor != null && r.Doctor.Name != null && r.Doctor.Name.Contains(doctorName)
                         && r.Patient != null && r.Patient.Name != null && r.Patient.Name.Contains(patientName))
               .ToListAsync();
        }

        public async Task<List<ReportDoctorToPatient>> GetAllReportwroteWhilePatientAppointsToRoom(string PatientId, int AppointmentToRoomId)
        {
            var app = await _context.AppointmentToRooms.IgnoreQueryFilters().Include(a => a.Room).FirstOrDefaultAsync(a => a.Id == AppointmentToRoomId && a.PatientId == PatientId);
            if (app == null || app.StartTime == null) return null;
            var reports = await _context.ReportDoctorToPatients
                 .IgnoreQueryFilters()
                 .Include(r => r.Patient)
                 .Include(r => r.Doctor)
                 .Where(r => r.PatientId == PatientId && DateOnly.FromDateTime(r.CreatedAt) <= DateOnly.FromDateTime(app.EndTime == null ? EgyptTime.Now : app.EndTime.Value) && DateOnly.FromDateTime(r.CreatedAt) >= DateOnly.FromDateTime(app.StartTime.Value))
                 .ToListAsync();
            return reports;
        }

        public async Task<List<GetPatientsDto>> GetAllpatientsreportedbySpecificDoctor(string DoctorId)
        {
            var patients = await _context.ReportDoctorToPatients
                .Where(r => r.DoctorId == DoctorId)
                .Include(r => r.Patient)
                .ToListAsync();
            var patientsDto = new List<GetPatientsDto>();
            foreach (var report in patients)
            {
                patientsDto.Add(report.Patient.ToGetPatientsDto());
            }
            return patientsDto;
        }
    }
}