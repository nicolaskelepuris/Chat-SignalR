using Domain.Entities.Chats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.ModelConfigurations.Base;

namespace Persistence.ModelConfigurations.Chats
{
    public class ChatMessageModelConfiguration : BaseModelConfiguration<ChatMessage>
    {
        public override void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            base.Configure(builder);

            builder.HasOne(fk => fk.Sender)
                .WithMany()
                .HasForeignKey(fk => fk.SenderId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}