namespace Safi.Dto.Bill
{
    public class BillDto
    {
        public int id { get; set; }
        public DateTime st_Date { get; set; }
        public DateTime? end_Date { get; set; }
        public string Status { get; set; }   // "open" , "closed"
        public string? Details { get; set; }
        public string currency { get; set; }
        public decimal TotalAmount { get; set; }
        public string PatientId { get; set; }
        public string PatientName { get; set; }
    }
}
