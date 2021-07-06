using System.ComponentModel.DataAnnotations;
using Application.Responses.Auth.Logins;
using MediatR;

namespace Application.Requests.Auth.Logins
{
    public class LoginRequest : IRequest<LoginResponse>
    {
        [Required(ErrorMessage = "Email é obrigatorio")]
        [EmailAddress(ErrorMessage = "Email invalido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é obrigatoria")]
        public string Password { get; set; }
    }
}