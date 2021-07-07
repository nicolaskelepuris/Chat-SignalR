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