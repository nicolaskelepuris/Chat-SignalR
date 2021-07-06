using System;
using System.Linq;
using System.Linq.Expressions;
using Domain.Entities.Chats;
using Domain.Specifications.Base;

namespace Domain.Specifications.Chats
{
    public class GetChatRoomsForUserSpecification : BaseSpecification<ChatRoom>
    {
        public GetChatRoomsForUserSpecification(string userId)
        {
            Criteria = CreateCriteria(userId);
        }

        private static Expression<Func<ChatRoom, bool>> CreateCriteria(string userId)
        {
            return chatRoom =>
                chatRoom.Users.Any(u => u.UserId == userId && (!u.ClosedChat || u.HasNewMessage));
        }
    }
}