using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Messages.Chats;

public interface IChatsStorage
{
    Task<(IEnumerable<Chat>, long)> SearchAsync(int slip, int take, Guid userId);

    Task<Chat> GetByIdAsync(Guid id);

    Task<Chat> CreateAsync(Chat chat);

    Task UpdateAsync(Chat chat);

    Task DeleteAsync(Guid id);

    Task ReorderAsync(Guid id);
}