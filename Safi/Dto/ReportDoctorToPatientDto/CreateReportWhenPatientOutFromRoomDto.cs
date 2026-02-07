namespace Safi.Dto.ReportDoctorToPatientDto
{
    public class CreateReportWhenPatientGetOutRoomDto
    {
        public int id { get; set; }    //AppointmentToRoomId
        public DateTime EndTime { get; set; }
        public string Report { get; set; }
        public string? CreatedBy { get; set; }
        public List<string>? Medicines { get; set; } = new List<string>();

    }
}
