using System.Text.Json.Serialization;

namespace Application.Responses.Base
{
    public class BaseResponse
    {
        [JsonIgnore]
        public int? StatusCode { get; set; }
        [JsonIgnore]
        public string ErrorMessage { get; set; }
    }
}