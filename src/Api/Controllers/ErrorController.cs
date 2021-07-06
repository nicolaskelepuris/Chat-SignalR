using Application.Responses;
using Application.Responses.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("errors/{statusCode}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        public IActionResult Error(int statusCode)
        {
            return new ObjectResult(new ApiResponse<string>
            {
                Success = false,
                Data = null,
                Error = new ErrorResponse(statusCode)
            });
        }
    }
}