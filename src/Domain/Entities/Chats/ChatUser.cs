using System;
using Domain.Entities.Base;

namespace Domain.Entities.Chats
{
    public class ChatUser : BaseEntity
    {
        public AppUser User { get; set; }
        public string UserId { get; set; }
        public ChatRoom Room { get; set; }
        public Guid RoomId { get; set; }
        public bool ClosedChat { get; set; }
        public bool HasNewMessage { get; set; }
    }
}