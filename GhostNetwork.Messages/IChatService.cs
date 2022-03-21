using System;
using System.Threading.Tasks;

namespace GhostNetwork.Messages
{
    public interface IChatService
    {
        Task SendMessage(Guid from, Guid to, Message message);

        Task ConnectToChat(Guid chatId);

        Task DeleteMessage(Guid senderId, Guid chatId, Guid messageId);

        Task UpdateMessage(Guid senderId, Guid chatId, Guid messageId, Message updateMessage);
    }
}