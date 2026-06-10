using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Safi.Helpers;

namespace Safi.Models
{
    public class OutboxMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Type { get; set; } = string.Empty; // "BillAndEmailForReservation" 
        [Required]
        public string Payload { get; set; } = string.Empty; // JSON
        public DateTime CreatedAt { get; set; } = EgyptTime.Now;
        public DateTime? ProcessedAt { get; set; }
        public DateTime? FailedAt { get; set; }
        public string? Error { get; set; }
        public int RetryCount { get; set; }=0;
    }
}

