using System.ComponentModel.DataAnnotations;

namespace Safi.Dto.Prices
{
    public class CreatePriceDto
    {
        [Required]
        [MaxLength(100)]
        public string ServiceName { get; set; }
        [Required]
        [Range(typeof(decimal), "0", "99999999.99")]
        public decimal Price { get; set; }
        public DateOnly? St_Date { get; set; }
        public DateOnly? End_Date { get; set; }
    }
}
