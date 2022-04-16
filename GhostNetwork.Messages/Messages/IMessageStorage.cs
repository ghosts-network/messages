using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Messages.Messages;

public interface IMessageStorage
{
    Task<(IEnumerable<Message>, long)> SearchAsync(int skip, int take, Guid chatId);

    Task<Message> GetByIdAsync(Guid id);

    Task<Message> SendAsync(Message message);

    Task DeleteAsync(Guid id);

    Task UpdateAsync(Guid id, string message);
}