using System;
using System.Linq.Expressions;
using Domain.Entities.Chats;
using Domain.Specifications.Base;

namespace Domain.Specifications.Chats
{
    public class GetChatUserByUserIdAndRoomId : BaseSpecification<ChatUser>
    {
        public GetChatUserByUserIdAndRoomId(string userId, Guid roomId)
        {
            Criteria = CreateCriteria(userId, roomId);
        }

        private static Expression<Func<ChatUser, bool>> CreateCriteria(string userId, Guid roomId)
        {
            return chatUser =>
                chatUser.RoomId == roomId
                &&
                chatUser.UserId == userId ;
        }
    }
}