using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace GhostNetwork.Messages.Chats;

public interface IChatsStorage
{
    Task<IEnumerable<Chat>> SearchAsync(Filter filter, Pagination pagination);

    Task<Chat> GetByIdAsync(ObjectId id);

    Task<Chat> CreateAsync(Chat chat);

    Task UpdateAsync(Chat chat);

    Task<bool> DeleteAsync(ObjectId id);

    Task ReorderAsync(ObjectId id);
}