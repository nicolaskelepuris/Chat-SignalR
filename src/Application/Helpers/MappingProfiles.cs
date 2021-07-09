using System.Linq;
using Application.Requests.Users;
using Application.Responses.Chats;
using Application.Responses.Users;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Chats;

namespace Application.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            User();
            Chat();
        }

        private void User()
        {
            CreateMap<AppUser, UserResponse>();

            CreateMap<PostUserRequest, AppUser>()
                .ForMember(p => p.UserName, v => v.MapFrom(p => p.Email));
        }

        private void Chat(){
            CreateMap<ChatMessage, ChatMessageResponse>();
            CreateMap<ChatRoom, ChatRoomResponse>()
                .ForMember(p => p.Users, v => v.MapFrom(p => p.Users.Select(u => u.User)));
        }
    }
}