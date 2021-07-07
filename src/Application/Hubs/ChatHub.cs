using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Responses.Chats;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Chats;
using Domain.Enums;
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
        private readonly IMapper _mapper;

        public ChatHub(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
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

        public async Task SendMessage(string message, Guid roomId)
        {
            var sender = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(Context.User);

            var chatRoom = await _unitOfWork.Repository<ChatRoom>().GetEntityAsyncWithSpec(new GetChatRoomByIdForUserSpecification(sender.Id, roomId));

            if (chatRoom == null)
            {
                return;
            }

            var chatMessage = new ChatMessage()
            {
                Message = message,
                SentAt = DateTime.UtcNow,
                Sender = sender,
                RoomId = chatRoom.Id
            };

            switch (chatRoom.Type)
            {
                case ChatRoomTypes.Global:
                    await SendGlobalMessageAsync(chatMessage);
                    break;
                case ChatRoomTypes.Private:
                    break;
                default:
                    break;
            }
        }

        private async Task SendGlobalMessageAsync(ChatMessage chatMessage)
        {
            _unitOfWork.Repository<ChatMessage>().Add(chatMessage);

            await _unitOfWork.Complete();

            var response = _mapper.Map<ChatMessageResponse>(chatMessage);

            await Clients.OthersInGroup(chatMessage.RoomId.ToString()).SendAsync("ReceiveMessage", response, chatMessage.RoomId);

            response.IsSender = true;

            await Clients.Caller.SendAsync("ReceiveMessage", response, chatMessage.RoomId);
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