using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace GhostNetwork.Messages.Api.Hubs;

public class ChatHub : Hub<IChatHub>
{
    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task SendMessageAsync(Guid chatId, Guid senderId, Guid receiverId, string message)
    {
        var newMessage = Message.NewMessage(chatId, senderId, message);

        await _chatService.SendMessageAsync(newMessage);

        // TODO `from` -> currentUser?
        await Clients.Client(chatId.ToString()).SendMessageAsync(chatId, senderId, receiverId, message);
    }
}