namespace Safi.Dto.Statistics
{
    public class GeneralStatsDto
    {
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int AssignedDoctors { get; set; }
        public int UnassignedDoctors { get; set; }
    }

    public class DepartmentPatientStatsDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int PatientCount { get; set; }
    }

    public class PatientDistributionDto
    {
        public int PatientsInOneDepartment { get; set; }
        public int PatientsInMultipleDepartments { get; set; }
    }

    public class DepartmentRoomStatsDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int RoomCount { get; set; }
        public int IcuCount { get; set; }
        public int EmergencyCount { get; set; }
        public int AvailableRooms { get; set; }
        public int UnavailableRooms { get; set; }
    }

    public class SharedRoomStatsDto
    {
        public int RoomsWithOneDoctor { get; set; }
        public int RoomsWithMultipleDoctors { get; set; }
    }

    public class ResponsibleDoctorDto
    {
        public string DoctorId { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
    }
}
