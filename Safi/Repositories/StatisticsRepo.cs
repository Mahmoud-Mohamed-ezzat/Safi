using Microsoft.EntityFrameworkCore;
using Safi.Dto.Statistics;
using Safi.Interfaces;
using Safi.Models;

namespace Safi.Repositories
{
    public class StatisticsRepo : IStatisticsRepo
    {
        private readonly SafiContext _context;

        public StatisticsRepo(SafiContext context)
        {
            _context = context;
        }

        public async Task<GeneralStatsDto> GetGeneralStatsAsync()
        {
            var totalDoctors = await _context.Doctors.CountAsync();
            var totalPatients = await _context.Patients.CountAsync();

            var assignedDoctors = await _context.AssignRoomToDoctors
                .Where(a => a.EndDate == null || a.EndDate >= DateOnly.FromDateTime(DateTime.UtcNow)) // Active assignments
                .Select(a => a.DoctorId)
                .Distinct()
                .CountAsync();

            return new GeneralStatsDto
            {
                TotalDoctors = totalDoctors,
                TotalPatients = totalPatients,
                AssignedDoctors = assignedDoctors,
                UnassignedDoctors = totalDoctors - assignedDoctors
            };
        }

        public async Task<List<DepartmentPatientStatsDto>> GetPatientsPerDepartmentAsync()
        {
            return await _context.Departments
                .Select(d => new DepartmentPatientStatsDto
                {
                    DepartmentName = d.Name,
                    PatientCount = d.Patients!.Count
                })
                .ToListAsync();
        }

        public async Task<PatientDistributionDto> GetPatientDistributionAsync()
        {
            var oneDept = await _context.Patients.CountAsync(p => p.Departments!.Count == 1);
            var multiDept = await _context.Patients.CountAsync(p => p.Departments!.Count > 1);

            return new PatientDistributionDto
            {
                PatientsInOneDepartment = oneDept,
                PatientsInMultipleDepartments = multiDept
            };
        }

        public async Task<List<DepartmentRoomStatsDto>> GetRoomStatsPerDepartmentAsync()
        {
            // RoomType discriminators: "NormalRoom", "Icu", "Emergency"
            // RoomStatus: "Available"

            var stats = await _context.Departments
                .Select(d => new
                {
                    d.Name,
                    AllRooms = d.Rooms!.Count(), // Includes all types
                    NormalRooms = d.Rooms!.Count(r=>r.GetType().Name=="Room"),
                    Icus = d.Icus!.Count(),
                    Emergencies = d.Emergencies!.Count(),
                    Available = d.Rooms!.Count(r => r.Status == "Available"),
                    Unavailable = d.Rooms!.Count(r => r.Status != "Available")
                })
                .ToListAsync();

            return stats.Select(s => new DepartmentRoomStatsDto
            {
                DepartmentName = s.Name,
                RoomCount = s.NormalRooms,
                IcuCount = s.Icus,
                EmergencyCount = s.Emergencies,
                AvailableRooms = s.Available,
                UnavailableRooms = s.Unavailable
            }).ToList();
        }

        public async Task<SharedRoomStatsDto> GetSharedRoomStatsAsync()
        {
            // Rooms assigned to just one doctor vs more than one
            // We look at AssignRoomToDoctors grouped by RoomId (active assignments)

            var assignments = _context.AssignRoomToDoctors
                .Where(a => a.EndDate == null || a.EndDate >= DateOnly.FromDateTime(DateTime.UtcNow));

            var grouped = assignments.GroupBy(a => a.RoomId);

            var oneDoc = await grouped.CountAsync(g => g.Count() == 1);
            var multiDoc = await grouped.CountAsync(g => g.Count() > 1);

            return new SharedRoomStatsDto
            {
                RoomsWithOneDoctor = oneDoc,
                RoomsWithMultipleDoctors = multiDoc
            };
        }

        public async Task<List<ResponsibleDoctorDto>> GetResponsibleDoctorsForPatientAsync(string patientId)
        {
            // Get doctors from active appointments and history
            // We'll focus on AppointmentToRoom usage as requested

            var doctors = await _context.AppointmentToRooms
                .Include(a => a.Doctor)
                .ThenInclude(d => d!.Department)
                .Where(a => a.PatientId == patientId && a.Doctor != null)
                .Select(a => a.Doctor)
                .Distinct()
                .ToListAsync();

            return doctors.Select(d => new ResponsibleDoctorDto
            {
                DoctorId = d!.Id,
                DoctorName = d.UserName ?? "Unknown",
                DepartmentName = d.Department?.Name ?? "Unknown"
            }).ToList();
        }
    }
}
