using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace GhostNetwork.Messages.Api.Domain;

public interface IMessagesStorage
{
    Task<IReadOnlyCollection<Message>> SearchAsync(Filter filter, Pagination pagination);

    Task<Message> GetByIdAsync(string chatId, string id);

    Task InsertAsync(Message message);

    Task UpdateAsync(Message message);

    Task<bool> DeleteAsync(string chatId, string id);

    Task DeleteByChatAsync(string chatId);
}