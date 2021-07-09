using Application.Responses.Base;

namespace Application.Responses.Users
{
    public class UserResponse : BaseResponse
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Nickname { get; set; }
    }
}