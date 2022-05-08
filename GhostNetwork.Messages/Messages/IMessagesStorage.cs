using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Messages.Messages;

public interface IMessagesStorage
{
    Task<IEnumerable<Message>> SearchAsync(MessageFilter filter, Pagination pagination);

    Task<Message> GetByIdAsync(Id id);

    Task SendAsync(Message message);

    Task UpdateAsync(Message message);

    Task DeleteAsync(Id id);
}