using System;
using System.Linq;
using System.Linq.Expressions;
using Domain.Entities.Chats;
using Domain.Enums;
using Domain.Specifications.Base;

namespace Domain.Specifications.Chats
{
    public class GetPrivateChatRoomByUsersIdsSpecification : BaseSpecification<ChatRoom>
    {
        public GetPrivateChatRoomByUsersIdsSpecification(string userOneId, string userTwoId)
        {
            Criteria = CreateCriteria(userOneId, userTwoId);
        }

        private static Expression<Func<ChatRoom, bool>> CreateCriteria(string userOneId, string userTwoId)
        {
            return chatRoom =>
                chatRoom.Type == ChatRoomTypes.Private
                &&
                chatRoom.Users.Any(u => u.UserId == userOneId)
                &&
                chatRoom.Users.Any(u => u.UserId == userTwoId);
        }
    }
}