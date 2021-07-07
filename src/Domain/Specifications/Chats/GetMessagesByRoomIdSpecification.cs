using System;
using System.Linq.Expressions;
using Domain.Entities.Chats;
using Domain.Specifications.Base;

namespace Domain.Specifications.Chats
{
    public class GetMessagesByRoomIdSpecification : BaseSpecification<ChatMessage>
    {
        public GetMessagesByRoomIdSpecification(Guid roomId)
        {
            Criteria = CreateCriteria(roomId);
            ApplyPaging(0, 20);
            AddOrderByDescending(m => m.SentAt);
            AddInclude(m => m.Sender);
        }

        private static Expression<Func<ChatMessage, bool>> CreateCriteria(Guid roomId)
        {
            return chatMessage =>
                chatMessage.RoomId == roomId;
        }
    }
}