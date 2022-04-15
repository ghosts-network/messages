using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace GhostNetwork.Messages
{
    public interface IChatService
    {
        Task<(IEnumerable<Guid>, long)> SearchChatsAsync(int skip, int take, Guid userId);

        Task<Chat> GetChatByIdAsync(Guid chatId);

        Task<Guid> CreateNewChatAsync(IEnumerable<Guid> users);

        Task AddNewUsersToChatAsync(Guid chatId, IEnumerable<Guid> newUsers);

        Task DeleteChatAsync(Guid chatId);
    }

    public class ChatService : IChatService
    {
        private readonly IChatStorage _chatStorage;

        public ChatService(IChatStorage chatStorage)
        {
            _chatStorage = chatStorage;
        }

        public async Task<(IEnumerable<Guid>, long)> SearchChatsAsync(int skip, int take, Guid userId)
        {
            return await _chatStorage.SearchChatsAsync(skip, take, userId);
        }

        public async Task<Chat> GetChatByIdAsync(Guid chatId)
        {
            return await _chatStorage.GetChatByIdAsync(chatId);
        }

        public async Task<Guid> CreateNewChatAsync(IEnumerable<Guid> users)
        {
            var chat = Chat.NewChat(users);

            return await _chatStorage.CreateNewChatAsync(chat);
        }

        public async Task AddNewUsersToChatAsync(Guid chatId, IEnumerable<Guid> newUsers)
        {
            await _chatStorage.AddNewUsersToChatAsync(chatId, newUsers);
        }

        public async Task DeleteChatAsync(Guid chatId)
        {
            await _chatStorage.DeleteChatAsync(chatId);
        }
    }
}