using System.ComponentModel.DataAnnotations;
using Application.Responses.Auth.Logins;
using MediatR;

namespace Application.Requests.Auth.Logins
{
    public class LoginGoogleRequest : IRequest<LoginResponse>
    {
        [Required(ErrorMessage = "Token é obrigatorio")]
        public string Token { get; set; }
    }
}