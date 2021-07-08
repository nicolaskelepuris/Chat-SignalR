using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Requests.Chats.Rooms;
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
        public async Task<IActionResult> GetChatRoomById([FromQuery] PaginationParams pagination)
        {
            var request = new GetChatRoomsRequest()
            {
                Pagination = pagination
            };
            return await CreateResponse(async () => await _mediator.Send(request));
        }
    }
}