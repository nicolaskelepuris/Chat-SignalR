using System.Threading;
using System.Threading.Tasks;
using Application.Requests.Users;
using Application.Responses.Users;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Handlers.Users
{
    public class GetUserHandler : IRequestHandler<GetUserRequest, UserResponse>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        public GetUserHandler(UserManager<AppUser> userManager, IMapper mapper)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<UserResponse> Handle(GetUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);

            if (user == null)
            {
                return new UserResponse() { StatusCode = 404, ErrorMessage = "Usuario não encontrado" };
            }

            return _mapper.Map<UserResponse>(user);
        }
    }
}