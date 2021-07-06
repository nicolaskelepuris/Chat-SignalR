using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Chats;
using Domain.Extensions;
using Domain.Interfaces;
using Domain.Specifications.Chats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Application.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private static Dictionary<string, string> connectionMapping = new Dictionary<string, string>();
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public ChatHub(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public override async Task OnConnectedAsync()
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(Context.User);

            SaveUserInConnectionMapping(user.Id);

            var rooms = await _unitOfWork.Repository<ChatRoom>().ListAsyncWithSpec(new GetChatRoomsForUserSpecification(user.Id));

            foreach (var room in rooms)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, room.Id.ToString());
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(Context.User);

            RemoveUserFromConnectionMapping(user.Id);
        }

        private void RemoveUserFromConnectionMapping(string userId)
        {
            if (connectionMapping.ContainsKey(userId))
            {
                connectionMapping.Remove(userId);
            }
        }

        private void SaveUserInConnectionMapping(string userId)
        {
            if (connectionMapping.ContainsKey(userId))
            {
                connectionMapping[userId] = Context.ConnectionId;
            }
            else
            {
                connectionMapping.Add(userId, Context.ConnectionId);
            }
        }
    }
}