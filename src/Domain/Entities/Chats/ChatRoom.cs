using System.Collections.Generic;
using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities.Chats
{
    public class ChatRoom : BaseEntity
    {
        public ICollection<ChatUser> Users { get; set; }
        public ICollection<ChatMessage> Messages { get; set; }
        public ChatRoomTypes Type { get; set; }
    }
}