using System.Threading.Tasks;
using Application.Requests.Auth.Logins;
using Application.Responses;
using Application.Responses.Auth.Logins;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/auth")]
    [Authorize]
    public class AuthController : BaseController
    {
        public AuthController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("login-google")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), 200)]
        public async Task<IActionResult> LoginGoogle([FromBody] LoginGoogleRequest request)
        {
            return await CreateResponse(async () => await _mediator.Send(request));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), 200)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            return await CreateResponse(async () => await _mediator.Send(request));
        }
    }
}