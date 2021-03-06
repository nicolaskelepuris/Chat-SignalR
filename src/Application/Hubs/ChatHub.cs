using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Responses.Chats;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Chats;
using Domain.Enums;
using Domain.Extensions;
using Domain.Helpers;
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

        const string RECEIVE_MESSAGE_METHOD = "ReceiveMessage";
        const string OPEN_ROOM_METHOD = "OpenRoom";

        public ChatHub(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = ContextHelper.GetUserIdFromClaimsPrincipal(Context.User);

            SaveUserInConnectionMapping(userId);

            var rooms = await _unitOfWork.Repository<ChatRoom>().ListAsyncWithSpec(new GetChatRoomsForUserSpecification(userId));

            foreach (var room in rooms)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, room.Id.ToString());
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userId = ContextHelper.GetUserIdFromClaimsPrincipal(Context.User);

            RemoveUserFromConnectionMapping(userId);

            return Task.CompletedTask;
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
                Room = chatRoom,
                RoomId = chatRoom.Id
            };

            _unitOfWork.Repository<ChatMessage>().Add(chatMessage);

            switch (chatRoom.Type)
            {
                case ChatRoomTypes.Global:
                    await SendGlobalMessageAsync(chatMessage);
                    break;
                case ChatRoomTypes.Private:
                    await SendPrivateMessageAsync(chatMessage);
                    break;
                default:
                    break;
            }
        }

        public async Task LeavePrivateRoom(Guid roomId)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(Context.User);

            var chatRoom = await _unitOfWork.Repository<ChatRoom>().GetEntityAsyncWithSpec(new GetChatRoomByIdForUserSpecification(user.Id, roomId));

            if (chatRoom == null || chatRoom.Type == ChatRoomTypes.Global)
            {
                return;
            }

            var chatUser = await _unitOfWork.Repository<ChatUser>().GetEntityAsyncWithSpec(new GetChatUserByUserIdAndRoomId(user.Id, roomId));

            if (chatUser == null)
            {
                return;
            }

            chatUser.ClosedChat = true;
            chatUser.HasNewMessage = false;

            _unitOfWork.Repository<ChatUser>().Update(chatUser);
            await _unitOfWork.Complete();
        }

        public async Task JoinPrivateRoom(string targetUserId)
        {
            var user = await _userManager.FindUserByEmailAsyncFromClaimsPrincipal(Context.User);

            if (user.Id == targetUserId)
            {
                return;
            }

            var targetUser = await _userManager.FindByIdAsync(targetUserId);

            if (targetUser == null)
            {
                return;
            }

            var chatRoom = await _unitOfWork.Repository<ChatRoom>()
                .GetEntityAsyncWithSpec(new GetPrivateChatRoomByUsersIdsSpecification(user.Id, targetUser.Id));

            if (chatRoom == null)
            {
                chatRoom = await CreatePrivateRoomAsync(user, targetUser);
            }
            else
            {
                await OpenUserChatAsync(user.Id, chatRoom.Id);
            }

            await AddUserToGroupAsync(user.Id, chatRoom.Id);

            await AddUserToGroupAsync(targetUserId, chatRoom.Id);

            await SendOpenRoomMethodToCallerAsync(chatRoom.Id);
        }

        private async Task AddUserToGroupAsync(string userId, Guid roomId)
        {
            if (connectionMapping.ContainsKey(userId))
            {
                await Groups.AddToGroupAsync(connectionMapping[userId], roomId.ToString());
            }
        }

        private async Task OpenUserChatAsync(string userId, Guid roomId)
        {
            var chatUser = await _unitOfWork.Repository<ChatUser>().GetEntityAsyncWithSpec(new GetChatUserByUserIdAndRoomId(userId, roomId));

            if (chatUser != null)
            {
                chatUser.ClosedChat = false;
                _unitOfWork.Repository<ChatUser>().Update(chatUser);
                await _unitOfWork.Complete();
            }
        }

        private async Task<ChatRoom> CreatePrivateRoomAsync(AppUser sender, AppUser receiver)
        {
            var chatRoom = new ChatRoom()
            {
                Type = ChatRoomTypes.Private,
                Users = new List<ChatUser>()
                {
                    new ChatUser()
                    {
                        User = sender
                    },
                    new ChatUser()
                    {
                        User = receiver
                    }
                },
            };

            _unitOfWork.Repository<ChatRoom>().Add(chatRoom);
            await _unitOfWork.Complete();

            return chatRoom;
        }

        private async Task SendPrivateMessageAsync(ChatMessage chatMessage)
        {
            var receiverChatUser = await _unitOfWork.Repository<ChatUser>().GetEntityAsyncWithSpec(
                new GetMessageReceiverChatUserSpecification(chatMessage.SenderId, chatMessage.RoomId));

            if (receiverChatUser == null)
            {
                return;
            }

            var receiverClosedChat = receiverChatUser.ClosedChat;

            UpdateChatUserWithNewMessage(receiverChatUser);

            await _unitOfWork.Complete();

            if (receiverClosedChat)
            {
                await SendPrivateMessageToUserWithClosedChatAsync(chatMessage);
            }
            else
            {
                await SendReceiveMessageMethodToOthersInRoomAsync(chatMessage);
            }

            await SendReceiveMessageMethodToCallerAsync(chatMessage);
        }

        private async Task SendPrivateMessageToUserWithClosedChatAsync(ChatMessage chatMessage)
        {
            await SendOpenRoomMethodToOthersInRoomAsync(chatMessage.RoomId);
        }

        private async Task SendOpenRoomMethodToOthersInRoomAsync(Guid roomId)
        {
            await Clients.OthersInGroup(roomId.ToString()).SendAsync(OPEN_ROOM_METHOD, roomId);
        }

        private async Task SendOpenRoomMethodToCallerAsync(Guid roomId)
        {
            await Clients.Caller.SendAsync(OPEN_ROOM_METHOD, roomId);
        }

        private async Task SendReceiveMessageMethodToCallerAsync(ChatMessage chatMessage)
        {
            var response = _mapper.Map<ChatMessageResponse>(chatMessage);
            response.IsSender = true;

            await Clients.Caller.SendAsync(RECEIVE_MESSAGE_METHOD, response, chatMessage.RoomId);
        }

        private async Task SendReceiveMessageMethodToOthersInRoomAsync(ChatMessage chatMessage)
        {
            var response = _mapper.Map<ChatMessageResponse>(chatMessage);

            await Clients.OthersInGroup(chatMessage.RoomId.ToString()).SendAsync(RECEIVE_MESSAGE_METHOD, response, chatMessage.RoomId);
        }

        private void UpdateChatUserWithNewMessage(ChatUser chatUser)
        {
            chatUser.HasNewMessage = true;
            chatUser.ClosedChat = false;
            _unitOfWork.Repository<ChatUser>().Update(chatUser);
        }

        private async Task SendGlobalMessageAsync(ChatMessage chatMessage)
        {
            await _unitOfWork.Complete();

            var response = _mapper.Map<ChatMessageResponse>(chatMessage);

            await SendReceiveMessageMethodToOthersInRoomAsync(chatMessage);

            await SendReceiveMessageMethodToCallerAsync(chatMessage);
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