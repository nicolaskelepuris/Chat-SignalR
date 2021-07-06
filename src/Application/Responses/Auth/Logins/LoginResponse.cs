using Application.Responses.Base;
using Application.Responses.Users;

namespace Application.Responses.Auth.Logins
{
    public class LoginResponse : BaseResponse
    {
        public UserResponse User { get; set; }
        public string Token { get; set; }
    }
}