using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace GhostNetwork.Messages.Api.Domain;

public interface IMessagesStorage
{
    Task<IReadOnlyCollection<Message>> SearchAsync(Filter filter, Pagination pagination);

    Task<Message> GetByIdAsync(ObjectId chatId, ObjectId id);

    Task InsertAsync(Message message);

    Task UpdateAsync(Message message);

    Task<bool> DeleteAsync(ObjectId chatId, ObjectId id);

    Task DeleteByChatAsync(ObjectId chatId);
}