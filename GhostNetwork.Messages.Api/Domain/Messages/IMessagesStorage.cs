using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace GhostNetwork.Messages.Api.Domain;

public interface IMessagesStorage
{
    Task<IEnumerable<Message>> SearchAsync(Filter filter, Pagination pagination);

    Task<Message> GetByIdAsync(ObjectId id);

    Task SendAsync(Message message);

    Task UpdateAsync(Message message);

    Task<bool> DeleteAsync(ObjectId id);

    Task DeleteByChatAsync(ObjectId chatId);
}