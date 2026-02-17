namespace Safi.Dto.AppointmentToRoom
{
    public class AssignedDoctorDto
    {
        public string DoctorId { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;
        public float Rank { get; set; }
    }
}
