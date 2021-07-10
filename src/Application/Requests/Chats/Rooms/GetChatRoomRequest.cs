using System;
using Application.Responses.Chats;
using MediatR;

namespace Application.Requests.Chats.Rooms
{
    public class GetChatRoomRequest : IRequest<ChatRoomResponse>
    {
        public Guid Id { get; set; }
    }
}