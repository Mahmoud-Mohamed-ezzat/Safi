using System.ComponentModel.DataAnnotations.Schema;

namespace Safi.Dto.Bill
{
    public class UpdateBillDto
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }
        public string? PatientId { get; set; }
        public string Details { get; set; }
    }
}
