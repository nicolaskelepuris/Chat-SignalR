using System.ComponentModel.DataAnnotations;
using Application.Responses.Users;
using MediatR;

namespace Application.Requests.Users
{
    public class PostUserRequest : IRequest<UserResponse>
    {
        [Required(ErrorMessage = "Nickname é obrigatorio")]
        public string Nickname { get; set; }

        [Required(ErrorMessage = "Email é obrigatorio")]
        [EmailAddress(ErrorMessage = "Email invalido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é obrigatoria")]
        public string Password { get; set; }
    }
}