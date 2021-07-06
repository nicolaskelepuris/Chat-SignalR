using System;
using Domain.Entities;
using Domain.Entities.Chats;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Persistence.ModelConfigurations.Chats;

namespace Persistence.Identity
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options)
        {
        }

        public virtual DbSet<ChatRoom> ChatRooms { get; set; }
        public virtual DbSet<ChatMessage> ChatMessages { get; set; }        
        public virtual DbSet<ChatUser> ChatUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));

            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new ChatRoomModelConfiguration());
            builder.ApplyConfiguration(new ChatMessageModelConfiguration());
            builder.ApplyConfiguration(new ChatUserModelConfiguration());
        }
    }
}