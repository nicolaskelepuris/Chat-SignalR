using Domain.Entities.Chats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.ModelConfigurations.Base;

namespace Persistence.ModelConfigurations.Chats
{
    public class ChatUserModelConfiguration : BaseModelConfiguration<ChatUser>
    {
        public override void Configure(EntityTypeBuilder<ChatUser> builder)
        {
            base.Configure(builder);

            builder.HasOne(fk => fk.User)
                .WithMany()
                .HasForeignKey(fk => fk.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasOne(fk => fk.Room)
                .WithMany(fk => fk.Users)
                .HasForeignKey(fk => fk.RoomId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}