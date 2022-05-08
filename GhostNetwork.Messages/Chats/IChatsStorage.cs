using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Messages.Chats;

public interface IChatsStorage
{
    Task<IEnumerable<Chat>> SearchAsync(ChatFilter filter, Pagination pagination);

    Task<Chat> GetByIdAsync(Id id);

    Task<Chat> CreateAsync(Chat chat);

    Task UpdateAsync(Chat chat);

    Task DeleteAsync(Id id);

    Task ReorderAsync(Id id);
}