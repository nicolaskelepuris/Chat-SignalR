using System.Linq;
using System.Security.Claims;

namespace Domain.Helpers
{
    public static class ContextHelper
    {
        public static string GetUserIdFromClaimsPrincipal(ClaimsPrincipal user)
        {
            var userId = user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }
    }
}