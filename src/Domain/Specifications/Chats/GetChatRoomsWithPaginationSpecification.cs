using System;
using System.Linq;
using System.Linq.Expressions;
using Domain.Entities.Chats;
using Domain.Enums;
using Domain.Specifications.Base;
using Domain.Utils;

namespace Domain.Specifications.Chats
{
    public class GetChatRoomsWithPaginationSpecification : BaseSpecification<ChatRoom>
    {
        public GetChatRoomsWithPaginationSpecification(PaginationParams paginationParams, string userId)
        {
            Criteria = CreateCriteria(userId);
            AddInclude("Users.User");
            ApplyPaging(paginationParams.PageSize * (paginationParams.PageIndex - 1), paginationParams.PageSize);
        }

        private static Expression<Func<ChatRoom, bool>> CreateCriteria(string userId)
        {
            return chatRoom =>
                chatRoom.Users.Any(u => u.UserId == userId && (!u.ClosedChat || u.HasNewMessage))
                ||
                chatRoom.Type == ChatRoomTypes.Global;
        }
    }
}