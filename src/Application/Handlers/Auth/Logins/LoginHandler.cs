using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Requests.Auth.Logins;
using Application.Responses.Auth.Logins;
using Application.Responses.Users;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Handlers.Auth.Logins
{
    public class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly SignInManager<AppUser> _signInManager;
        public LoginHandler(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper, SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
            _mapper = mapper;
            _tokenService = tokenService;
            _userManager = userManager;
        }

        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return new LoginResponse() { StatusCode = 200, ErrorMessage = "Usuario não encontrado" };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    return new LoginResponse() { StatusCode = 200, ErrorMessage = "Usuario está bloqueado por 5 minutos" };
                }
                else
                {
                    return new LoginResponse() { StatusCode = 200, ErrorMessage = "Usuario não encontrado" };
                }
            }


            return new LoginResponse()
            {
                User = _mapper.Map<UserResponse>(user),
                Token = _tokenService.CreateToken(user)
            };
        }
    }
}