using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Requests.Chats.Rooms.Messages;
using Application.Responses.Chats;
using Application.Responses.Pagination;
using AutoMapper;
using Domain.Entities.Chats;
using Domain.Helpers;
using Domain.Interfaces;
using Domain.Specifications.Chats;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Handlers.Chats.Rooms.Messages
{
    public class GetChatMessagesHandler : IRequestHandler<GetChatMessagesRequest, PaginationResponse<ChatMessageResponse>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetChatMessagesHandler(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<PaginationResponse<ChatMessageResponse>> Handle(GetChatMessagesRequest request, CancellationToken cancellationToken)
        {
            var userId = ContextHelper.GetUserIdFromClaimsPrincipal(_httpContextAccessor.HttpContext.User);

            var chatRoom = await _unitOfWork.Repository<ChatRoom>().GetEntityAsyncWithSpec(new GetChatRoomByIdForUserSpecification(userId, request.RoomId));

            if (chatRoom == null)
            {
                return new PaginationResponse<ChatMessageResponse>()
                {
                    StatusCode = 403,
                    ErrorMessage = "Usuario não tem acesso à essas mensagens ou sala de chat não existe"
                };
            }

            var chatMessages = await _unitOfWork.Repository<ChatMessage>().ListAsyncWithSpec(new GetChatMessagesWithPaginationSpecification(request.Pagination, request.RoomId));

            var count = await _unitOfWork.Repository<ChatMessage>().CountAsync(new GetChatMessagesForCountSpecification(request.RoomId));

            return new PaginationResponse<ChatMessageResponse>()
            {
                Items = _mapper.Map<IReadOnlyList<ChatMessageResponse>>(chatMessages),
                Page = request.Pagination.PageIndex,
                PageSize = request.Pagination.PageSize,
                ItemsCount = count
            };
        }
    }
}