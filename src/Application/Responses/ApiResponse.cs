using Application.Responses.Errors;

namespace Application.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public ErrorResponse Error { get; set; }
    }
}