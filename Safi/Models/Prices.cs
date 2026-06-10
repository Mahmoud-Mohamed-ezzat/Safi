using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Safi.Helpers;

namespace Safi.Models
{
    public class Prices
    {
 [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string ServiceName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Required]
        public DateOnly St_Date { get; set; } 

        public DateOnly? End_Date { get; set; } = null;

        // Admin soft-delete only. Expired prices keep IsDeleted = false.
        public bool Is_Deleted { get; set; } = false;
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = EgyptTime.Now;
    }
}