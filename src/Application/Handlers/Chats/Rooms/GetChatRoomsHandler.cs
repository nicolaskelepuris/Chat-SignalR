using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Requests.Chats.Rooms;
using Application.Responses.Chats;
using Application.Responses.Pagination;
using AutoMapper;
using Domain.Entities.Chats;
using Domain.Helpers;
using Domain.Interfaces;
using Domain.Specifications.Chats;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Handlers.Chats.Rooms
{
    public class GetChatRoomsHandler : IRequestHandler<GetChatRoomsRequest, PaginationResponse<ChatRoomResponse>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetChatRoomsHandler(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaginationResponse<ChatRoomResponse>> Handle(GetChatRoomsRequest request, CancellationToken cancellationToken)
        {
            var userId = ContextHelper.GetUserIdFromClaimsPrincipal(_httpContextAccessor.HttpContext.User);

            var chatRooms = await _unitOfWork.Repository<ChatRoom>().ListAsyncWithSpec(new GetChatRoomsWithPaginationSpecification(request.Pagination, userId));

            var count = await _unitOfWork.Repository<ChatRoom>().CountAsync(new GetChatRoomsForCountSpecification(userId));

            return new PaginationResponse<ChatRoomResponse>()
            {
                Items = _mapper.Map<IReadOnlyList<ChatRoomResponse>>(chatRooms),
                Page = request.Pagination.PageIndex,
                PageSize = request.Pagination.PageSize,
                ItemsCount = count
            };
        }
    }
}