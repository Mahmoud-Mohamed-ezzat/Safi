using System.ComponentModel.DataAnnotations.Schema;

namespace Safi.Dto.Bill
{
    public class createbillDto
    {
        public DateTime st_Date { get; set; } = DateTime.UtcNow;
        public DateTime? end_Date { get; set; }
        public string Status { get; set; }   // "open" , "closed"
        public string? Details { get; set; }
        public string currency { get; set; }
        public decimal TotalAmount { get; set; }
        public string PatientId { get; set; }
    }
}
