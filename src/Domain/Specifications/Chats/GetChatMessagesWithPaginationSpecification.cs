using System;
using System.Linq.Expressions;
using Domain.Entities.Chats;
using Domain.Specifications.Base;
using Domain.Utils;

namespace Domain.Specifications.Chats
{
    public class GetChatMessagesWithPaginationSpecification : BaseSpecification<ChatMessage>
    {
        public GetChatMessagesWithPaginationSpecification(PaginationParams paginationParams, Guid roomId)
        {
            Criteria = CreateCriteria(roomId);
            AddInclude(p => p.Sender);
            ApplyPaging(paginationParams.PageSize * (paginationParams.PageIndex - 1), paginationParams.PageSize);
            AddOrderByDescending(p => p.SentAt);
        }

        private static Expression<Func<ChatMessage, bool>> CreateCriteria(Guid roomId)
        {
            return chatMessage =>
                chatMessage.RoomId == roomId;
        }
    }
}