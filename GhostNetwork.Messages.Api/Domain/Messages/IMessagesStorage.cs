using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Messages.Domain;

public interface IMessagesStorage
{
    Task<(IReadOnlyCollection<Message>, long)> SearchAsync(Filter filter, Pagination pagination);

    Task<Message> GetByIdAsync(string chatId, string id);

    Task InsertAsync(Message message);

    Task UpdateAsync(Message message);

    Task<bool> DeleteAsync(string chatId, string id);

    Task DeleteByChatAsync(string chatId);
}