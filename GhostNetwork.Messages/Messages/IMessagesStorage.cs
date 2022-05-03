using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Messages.Messages;

public interface IMessagesStorage
{
    Task<(IEnumerable<Message>, long, string)> SearchAsync(string lastMessageId, int take, Guid chatId);

    Task<Message> GetByIdAsync(string id);

    Task<string> SendAsync(Message message);

    Task DeleteAsync(string id);

    Task UpdateAsync(string id, string message);
}