using System;
using Application.Responses.Chats;
using Application.Responses.Pagination;
using Domain.Utils;
using MediatR;

namespace Application.Requests.Chats.Rooms.Messages
{
    public class GetChatMessagesRequest : IRequest<PaginationResponse<ChatMessageResponse>>
    {
        public PaginationParams Pagination { get; set; }
        public Guid RoomId { get; set; }
    }
}