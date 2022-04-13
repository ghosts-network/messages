using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Messages;

public interface IMessageStorage
{
    Task<(IEnumerable<Message>, long)> GetChatHistoryAsync(int skip, int take, Guid chatId);

    Task<Message> GetMessageByIdAsync(Guid id);

    Task<Message> SendMessageAsync(Message message);

    Task DeleteMessageAsync(Guid id);

    Task UpdateMessageAsync(Guid id, string message);
}