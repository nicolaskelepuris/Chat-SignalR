using Application.Requests.Users;
using Application.Responses.Chats;
using Application.Responses.Users;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Chats;
using Google.Apis.Auth;

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

            CreateMap<GoogleJsonWebSignature.Payload, AppUser>()
                .ForMember(p => p.EmailConfirmed, v => v.MapFrom(p => p.EmailVerified))
                .ForMember(p => p.UserName, v => v.MapFrom(p => p.Email));

            CreateMap<PostUserRequest, AppUser>()
                .ForMember(p => p.UserName, v => v.MapFrom(p => p.Email));
        }

        private void Chat(){
            CreateMap<ChatMessage, ChatMessageResponse>();
        }
    }
}