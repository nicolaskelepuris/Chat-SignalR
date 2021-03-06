using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Domain.Extensions
{
    public static class UserManagerExtension
    {
        public static async Task<AppUser> FindUserByEmailAsyncFromClaimsPrincipal(this UserManager<AppUser> userManager, ClaimsPrincipal user)
        {
            var email = user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            return await userManager.FindByEmailAsync(email);
        }
    }
}