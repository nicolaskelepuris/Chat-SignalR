using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Requests.Chats.Rooms;
using Application.Requests.Chats.Rooms.Messages;
using Application.Responses;
using Application.Responses.Chats;
using Application.Responses.Pagination;
using Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/chat-rooms")]
    [Authorize]
    public class ChatRoomsController : BaseController
    {
        public ChatRoomsController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginationResponse<ChatRoomResponse>>), 200)]
        public async Task<IActionResult> GetChatRooms([FromQuery] PaginationParams pagination)
        {
            var request = new GetChatRoomsRequest()
            {
                Pagination = pagination
            };
            return await CreateResponse(async () => await _mediator.Send(request));
        }

        [HttpGet("{id}/messages")]
        [ProducesResponseType(typeof(ApiResponse<PaginationResponse<ChatMessageResponse>>), 200)]
        public async Task<IActionResult> GetChatMessages([FromQuery] PaginationParams pagination, [FromRoute] Guid id)
        {
            var request = new GetChatMessagesRequest()
            {
                Pagination = pagination,
                RoomId = id
            };
            return await CreateResponse(async () => await _mediator.Send(request));
        }
    }
}