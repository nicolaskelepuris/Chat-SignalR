using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Chats;
using Domain.Enums;

namespace Persistence.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedAsync(AppIdentityDbContext context)
        {
            if (!context.ChatRooms.Any(c => c.Type == ChatRoomTypes.Global))
            {
                context.ChatRooms.Add(new ChatRoom()
                {
                    Type = ChatRoomTypes.Global
                });

                await context.SaveChangesAsync();
            }
        }
    }
}