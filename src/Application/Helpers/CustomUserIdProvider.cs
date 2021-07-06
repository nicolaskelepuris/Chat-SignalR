using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Application.Helpers
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public CustomUserIdProvider()
        {
        }

        public string GetUserId(HubConnectionContext connection)
        {
            var userId = connection.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }
    }
}