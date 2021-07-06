using System.Threading.Tasks;
using Application.Requests.Users;
using Application.Responses;
using Application.Responses.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/users")]
    [Authorize]
    public class UsersController : BaseController
    {
        public UsersController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<UserResponse>), 200)]
        public async Task<IActionResult> PostUser([FromBody] PostUserRequest request)
        {
            return await CreateResponse(async () => await _mediator.Send(request));
        }
    }
}