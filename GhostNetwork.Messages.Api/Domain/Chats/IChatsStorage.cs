using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Messages.Chats;

public interface IChatsStorage
{
    Task<IReadOnlyCollection<Chat>> SearchAsync(Filter filter, Pagination pagination);

    Task<Chat> GetByIdAsync(string id);

    Task<Chat> InsertAsync(Chat chat);

    Task UpdateAsync(Chat chat);

    Task<bool> DeleteAsync(string id);

    Task ReorderAsync(string id);
}