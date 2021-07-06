using System;
using Domain.Entities.Base;

namespace Domain.Entities.Chats
{
    public class ChatMessage : BaseEntity
    {
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
        public AppUser Sender { get; set; }
        public string SenderId { get; set; }
        public ChatRoom Room { get; set; }
        public Guid? RoomId { get; set; }
    }
}