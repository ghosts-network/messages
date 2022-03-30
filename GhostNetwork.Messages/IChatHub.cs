using System;
using System.Threading.Tasks;

namespace GhostNetwork.Messages;

public interface IChatHub
{
    Task SendMessageAsync(Guid chatId, Guid senderId, Guid receiverId, string message);
}