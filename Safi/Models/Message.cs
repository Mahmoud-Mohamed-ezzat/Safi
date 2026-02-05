using System.ComponentModel.DataAnnotations.Schema;

namespace Safi.Models
{
    public class Message
    {
        public int Id { get; set; }
        public required string MessageContent { get; set; }
        public bool IsEdited { get; set; }
        [ForeignKey("SenderId")]
        public virtual User? Sender { get; set; }
        public string? SenderId { get; set; }
        [ForeignKey("ReceiverId")]
        public virtual User? Receiver { get; set; }
        public string? ReceiverId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
