using System;
using System.Linq.Expressions;
using Domain.Entities.Chats;
using Domain.Specifications.Base;

namespace Domain.Specifications.Chats
{
    public class GetMessageReceiverChatUserSpecification : BaseSpecification<ChatUser>
    {
        public GetMessageReceiverChatUserSpecification(string senderId, Guid roomId)
        {
            Criteria = CreateCriteria(senderId, roomId);
        }

        private static Expression<Func<ChatUser, bool>> CreateCriteria(string senderId, Guid roomId)
        {
            return chatUser =>
                chatUser.RoomId == roomId
                &&
                chatUser.UserId != senderId;
        }
    }
}