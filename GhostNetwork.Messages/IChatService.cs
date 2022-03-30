using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Messages
{
    public interface IChatService
    {
        Task<(IEnumerable<Guid>, long)> SearchExistChatsAsync(int slip, int take, Guid userId);

        Task<Guid> GetExistChatByIdAsync(Guid chatId);

        Task<Guid> CreateNewChatAsync(Chat newChat);

        Task DeleteChatAsync(Guid chatId);

        Task<IEnumerable<Message>> GetChatHistoryAsync(Guid chatId);

        Task SendMessageAsync(Message message);

        Task DeleteMessageAsync(string messageId);

        Task UpdateMessageAsync(string messageId, string message);
    }
}