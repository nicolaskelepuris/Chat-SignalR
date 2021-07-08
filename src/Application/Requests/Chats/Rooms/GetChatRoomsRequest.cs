using System.Collections.Generic;
using Application.Responses.Chats;
using Application.Responses.Pagination;
using Domain.Utils;
using MediatR;

namespace Application.Requests.Chats.Rooms
{
    public class GetChatRoomsRequest : IRequest<PaginationResponse<ChatRoomResponse>>
    {
        public PaginationParams Pagination { get; set; }
    }
}