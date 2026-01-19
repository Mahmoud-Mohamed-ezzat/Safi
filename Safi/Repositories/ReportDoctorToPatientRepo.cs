using Microsoft.EntityFrameworkCore;
using Safi.Dto.ReportDoctorToPatientDto;
using Safi.Interfaces;
using Safi.Mapper;
using Safi.Models;

namespace Safi.Repositories
{
    public class ReportDoctorToPatientRepo : IReportDoctorToPatient
    {
        private readonly SafiContext _context;

        public ReportDoctorToPatientRepo(SafiContext context)
        {
            _context = context;
        }

        public async Task<List<ReportDoctorToPatient>> GetAllAsync()
        {
            return await _context.ReportDoctorToPatients
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ReportDoctorToPatient?> GetByIdAsync(int id)
        {
            return await _context.ReportDoctorToPatients
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<ReportDoctorToPatient> CreateAsync(CreateReportDoctorToPatientDto dto)
        {
            var report = dto.ToReportDoctorToPatient();
            report.CreatedAt = DateTime.Now;
            await _context.ReportDoctorToPatients.AddAsync(report);
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

        public async Task<List<ReportDoctorToPatient>> GetByPatientIdAsync(string patientId)
        {
            return await _context.ReportDoctorToPatients
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                .Where(r => r.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<List<ReportDoctorToPatient>> GetByDoctorIdAsync(string doctorId)
        {
            return await _context.ReportDoctorToPatients
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                .Where(r => r.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<List<ReportDoctorToPatient>> GetByDoctorIdAndDateAsync(string doctorId, DateTime date)
        {
            return await _context.ReportDoctorToPatients
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                .Where(r => r.DoctorId == doctorId && r.CreatedAt.Date == date.Date)
                .ToListAsync();
        }

        public async Task<List<ReportDoctorToPatient>> GetByMedicineAndPatientAsync(string medicine, string patientId)
        {
            // Optimized to filter in the database using EF Core's primitive collection support
            return await _context.ReportDoctorToPatients
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                .Where(r => r.PatientId == patientId && r.Medicines != null && r.Medicines.Contains(medicine))
                .ToListAsync();
        }

        public async Task<List<ReportDoctorToPatient>> GetByPatientNameAsync(string patientName)
        {
            return await _context.ReportDoctorToPatients
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                .Where(r => r.Patient != null && r.Patient.Name != null && r.Patient.Name.Contains(patientName))
                .ToListAsync();
        }

        public async Task<List<ReportDoctorToPatient>> GetByDoctorNameAsync(string doctorName)
        {
            return await _context.ReportDoctorToPatients
                .Include(r => r.Doctor)
                .Include(r => r.Patient)
                .Where(r => r.Doctor != null && r.Doctor.Name != null && r.Doctor.Name.Contains(doctorName))
                .ToListAsync();
        }

        public async Task<List<ReportDoctorToPatient>> GetByDoctorNameandPatientNameAsync(string doctorName, string patientName)
        {
            return await _context.ReportDoctorToPatients
               .Include(r => r.Doctor)
               .Include(r => r.Patient)
               .Where(r => r.Doctor != null && r.Doctor.Name != null && r.Doctor.Name.Contains(doctorName)
                         && r.Patient != null && r.Patient.Name != null && r.Patient.Name.Contains(patientName))
               .ToListAsync();
        }
    }
}
