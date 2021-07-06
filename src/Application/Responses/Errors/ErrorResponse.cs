namespace Application.Responses.Errors
{
    public class ErrorResponse
    {
        public ErrorResponse(int statusCode = 0, string message = null)
        {
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }
        public string Message { get; set; }
        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad request",
                401 => "Not authorized",
                403 => "Forbidden",
                404 => "No content found",
                500 => "Internal error",
                _ => null
            };
        }
    }
}