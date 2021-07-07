using System;
using Application.Responses.Users;

namespace Application.Responses.Chats
{
    public class ChatMessageResponse
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
        public UserResponse Sender { get; set; }
        public bool IsSender { get; set; }
    }
}