using System;
using System.Linq;
using System.Linq.Expressions;
using Domain.Entities.Chats;
using Domain.Specifications.Base;

namespace Domain.Specifications.Chats
{
    public class GetChatRoomByIdForUserSpecification : BaseSpecification<ChatRoom>
    {
        public GetChatRoomByIdForUserSpecification(string userId, Guid roomId)
        {
            Criteria = CreateCriteria(userId, roomId);
        }

        private static Expression<Func<ChatRoom, bool>> CreateCriteria(string userId, Guid roomId)
        {
            return chatRoom =>
                chatRoom.Id == roomId
                &&
                chatRoom.Users.Any(u => u.UserId == userId && (!u.ClosedChat || u.HasNewMessage));
        }
    }
}