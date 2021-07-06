using Domain.Entities.Chats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.ModelConfigurations.Base;

namespace Persistence.ModelConfigurations.Chats
{
    public class ChatRoomModelConfiguration : BaseModelConfiguration<ChatRoom>
    {
        public override void Configure(EntityTypeBuilder<ChatRoom> builder)
        {
            base.Configure(builder);

            builder.HasMany(fk => fk.Messages)
                .WithOne(fk => fk.Room)
                .HasForeignKey(fk => fk.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}