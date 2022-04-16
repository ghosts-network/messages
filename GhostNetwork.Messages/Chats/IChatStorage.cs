using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Messages.Chats;

public interface IChatStorage
{
    Task<(IEnumerable<Chat>, long)> SearchAsync(int slip, int take, Guid userId);

    Task<Chat> GetByIdAsync(Guid id);

    Task<Guid> CreatAsync(Chat chat);

    Task UpdateAsync(Chat chat);

    Task DeleteAsync(Guid id);
}