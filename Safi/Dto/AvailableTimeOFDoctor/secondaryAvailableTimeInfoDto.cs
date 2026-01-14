public class SecondaryAvailableTimeInfoDto
{
    public int Id { get; set; }
    public string DoctorName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateOnly Day { get; set; }
    public int Slots { get; set; }
}

