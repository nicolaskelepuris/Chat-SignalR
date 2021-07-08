using System;
using System.ComponentModel.DataAnnotations;
using Application.Responses.Users;
using MediatR;

namespace Application.Requests.Users
{
    public class GetUserRequest : IRequest<UserResponse>
    {
        [Required(ErrorMessage = "Id é obrigatorio")]
        public string Id { get; set; }
    }
}