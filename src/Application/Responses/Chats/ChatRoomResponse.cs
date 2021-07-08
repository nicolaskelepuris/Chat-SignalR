using System;
using System.Collections.Generic;
using Application.Responses.Users;
using Domain.Enums;

namespace Application.Responses.Chats
{
    public class ChatRoomResponse
    {
        public Guid Id { get; set; }
        public List<UserResponse> Users { get; set; }
        public ChatRoomTypes Type { get; set; }
    }
}