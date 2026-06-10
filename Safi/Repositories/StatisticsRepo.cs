using Microsoft.EntityFrameworkCore;
using Safi.Dto.Statistics;
using Safi.Helpers;
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
            var totalDoctors = await _context.Doctors.IgnoreQueryFilters().CountAsync();
            var totalPatients = await _context.Patients.IgnoreQueryFilters().CountAsync();

            var assignedDoctors = await _context.AssignWorks
                .IgnoreQueryFilters()
                .Where(a => a.EndDate == null || a.EndDate >= EgyptTime.Today) // Active assignments
                .Select(a => a.userId)
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
            var departments = await _context.Departments
                .IgnoreQueryFilters()
                .ToListAsync();
            
            var patients = await _context.Patients
                .IgnoreQueryFilters()
                .Include(p => p.Departments)
                .ToListAsync();

            return departments.Select(d => new DepartmentPatientStatsDto
            {
                DepartmentName = d.Name,
                PatientCount = patients.Count(p => p.Departments != null && p.Departments.Any(dept => dept.Id == d.Id))
            }).ToList();
        }

        public async Task<PatientDistributionDto> GetPatientDistributionAsync()
        {
            var oneDept = await _context.Patients.IgnoreQueryFilters().CountAsync(p => p.Departments!.Count == 1);
            var multiDept = await _context.Patients.IgnoreQueryFilters().CountAsync(p => p.Departments!.Count > 1);

            return new PatientDistributionDto
            {
                PatientsInOneDepartment = oneDept,
                PatientsInMultipleDepartments = multiDept
            };
        }

        public async Task<List<DepartmentRoomStatsDto>> GetRoomStatsPerDepartmentAsync()
        {
            var stats = await _context.Departments
                .IgnoreQueryFilters()
                .Select(d => new
                {
                    d.Name,
                    NormalRooms = d.Rooms!.Where(r => EF.Property<string>(r, "RoomType") == "NormalRoom").Count(),
                    Icus = d.Rooms!.Where(r => EF.Property<string>(r, "RoomType") == "Icu").Count(),
                    Emergencies = d.Rooms!.Where(r => EF.Property<string>(r, "RoomType") == "Emergency").Count(),
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

            var assignments = _context.AssignWorks
                .IgnoreQueryFilters()
                .Where(a => a.EndDate == null || a.EndDate >= EgyptTime.Today);

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
                .IgnoreQueryFilters()
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

        private async Task<int> GetRoomCountAsync(string roomType, int departmentId)
        {
            return await _context.Rooms
                .Where(r => EF.Property<string>(r, "RoomType") == roomType && r.DepartmentId == departmentId && r.Status == RoomStatus.Available)
                .CountAsync();
        }

        public async Task<int> GetNumberofHeartICUs() => await GetRoomCountAsync("Icu", 1);
        public async Task<int> GetNumberofKidneyICUs() => await GetRoomCountAsync("Icu", 2);
        public async Task<int> GetNumberofLiverICus() => await GetRoomCountAsync("Icu", 3);

        public async Task<int> GetNumberofHeartRooms() => await GetRoomCountAsync("NormalRoom", 1);
        public async Task<int> GetNumberofKidneyRooms() => await GetRoomCountAsync("NormalRoom", 2);
        public async Task<int> GetNumberofLiverRooms() => await GetRoomCountAsync("NormalRoom", 3);

        public async Task<int> GetNumberofHeartEmergencies() => await GetRoomCountAsync("Emergency", 1);
        public async Task<int> GetNumberofKidneyEmergencies() => await GetRoomCountAsync("Emergency", 2);
        public async Task<int> GetNumberofLiverEmergencies() => await GetRoomCountAsync("Emergency", 3);
    }
}
