using System;
using System.Linq;
using System.Linq.Expressions;
using Domain.Entities.Chats;
using Domain.Enums;
using Domain.Specifications.Base;

namespace Domain.Specifications.Chats
{
    public class GetPrivateChatRoomByIdIncludingUsers : BaseSpecification<ChatRoom>
    {
        public GetPrivateChatRoomByIdIncludingUsers(string userId, Guid roomId)
        {
            Criteria = CreateCriteria(userId, roomId);
            AddInclude("Users.User");
        }

        private static Expression<Func<ChatRoom, bool>> CreateCriteria(string userId, Guid roomId)
        {
            return chatRoom =>
                chatRoom.Id == roomId
                &&
                chatRoom.Type == ChatRoomTypes.Private
                &&
                chatRoom.Users.Any(u => u.UserId == userId);
        }
    }
}