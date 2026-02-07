using Microsoft.AspNetCore.SignalR;
using Safi.Models;
using Safi.Helpers;
namespace Safi.Hubs
{
    public class MessageHub:Hub
    {
        private readonly SafiContext _context;

        public MessageHub(SafiContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        public async Task SendMessageToUser(string senderId, string receiverId, string messageContent)
        {
            //  Encrypt Before Save
            var encryptedText = AesChatHelper.Encrypt(messageContent);

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                MessageContent = encryptedText,
                CreatedAt = DateTime.Now
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            //  Decrypt Before Sending
            var decryptedText = messageContent;

            await Clients.Group(receiverId)
                .SendAsync("ReceiveMessage", message.Id, senderId, decryptedText);
        }



        public async Task EditMessage(int messageId, string newContent)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null) return;

            message.MessageContent = AesChatHelper.Encrypt(newContent);
            await _context.SaveChangesAsync();

            // Send decrypted text to clients
            await Clients.Users(message.SenderId, message.ReceiverId)
                       .SendAsync("MessageEdited", messageId, newContent);
        }



        public async Task DeleteMessage(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null) return;

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            await Clients.Users(message.SenderId, message.ReceiverId)
                       .SendAsync("MessageDeleted", messageId);
        }
    }
}
