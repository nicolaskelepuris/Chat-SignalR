using System.Threading;
using System.Threading.Tasks;
using Application.Requests.Chats.Rooms;
using Application.Responses.Chats;
using AutoMapper;
using Domain.Entities.Chats;
using Domain.Helpers;
using Domain.Interfaces;
using Domain.Specifications.Chats;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Handlers.Chats.Rooms
{
    public class GetChatRoomHandler : IRequestHandler<GetChatRoomRequest, ChatRoomResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetChatRoomHandler(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ChatRoomResponse> Handle(GetChatRoomRequest request, CancellationToken cancellationToken)
        {
            var userId = ContextHelper.GetUserIdFromClaimsPrincipal(_httpContextAccessor.HttpContext.User);

            var chatRoom = await _unitOfWork.Repository<ChatRoom>().GetEntityAsyncWithSpec(new GetPrivateChatRoomByIdIncludingUsers(userId, request.Id));

            return _mapper.Map<ChatRoomResponse>(chatRoom);
        }
    }
}