using System;
using System.Linq.Expressions;
using Domain.Entities.Chats;
using Domain.Specifications.Base;

namespace Domain.Specifications.Chats
{
    public class GetChatMessagesForCountSpecification : BaseSpecification<ChatMessage>
    {
        public GetChatMessagesForCountSpecification(Guid roomId)
        {
            Criteria = CreateCriteria(roomId);
        }

        private static Expression<Func<ChatMessage, bool>> CreateCriteria(Guid roomId)
        {
            return chatMessage =>
                chatMessage.RoomId == roomId;
        }
    }
}