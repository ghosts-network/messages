using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Messages;

public interface IChatStorage
{
    Task<(IEnumerable<Guid>, long)> SearchChatsAsync(int slip, int take, Guid userId);

    Task<Chat> GetChatByIdAsync(Guid chatId);

    Task<Guid> CreateNewChatAsync(Chat newChat);

    Task AddNewUsersToChatAsync(Guid chatId, IEnumerable<Guid> users);

    Task DeleteChatAsync(Guid chatId);
}