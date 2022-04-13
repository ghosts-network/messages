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

        Task<(IEnumerable<Message>, long)> GetChatHistoryAsync(int skip, int take, Guid chatId);

        Task<(DomainResult, Message)> SendMessageAsync(Guid chatId, Guid senderId, string data);

        Task DeleteMessageAsync(Guid id);

        Task<DomainResult> UpdateMessageAsync(Guid id, string data);
    }

    public class ChatService : IChatService
    {
        private readonly IChatStorage _chatStorage;
        private readonly IMessageStorage _messageStorage;

        public ChatService(IChatStorage chatStorage, IMessageStorage messageStorage)
        {
            _chatStorage = chatStorage;
            _messageStorage = messageStorage;
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

        public async Task<(IEnumerable<Message>, long)> GetChatHistoryAsync(int skip, int take, Guid chatId)
        {
            return await _messageStorage.GetChatHistoryAsync(skip, take, chatId);
        }

        public async Task<(DomainResult, Message)> SendMessageAsync(Guid chatId, Guid senderId, string data)
        {
            var newMessage = Message.NewMessage(chatId, senderId, data);

            var message = await _messageStorage.SendMessageAsync(newMessage);

            return (DomainResult.Success(), message);
        }

        public async Task DeleteMessageAsync(Guid id)
        {
            await _messageStorage.DeleteMessageAsync(id);
        }

        public async Task<DomainResult> UpdateMessageAsync(Guid id, string data)
        {
            await _messageStorage.UpdateMessageAsync(id, data);

            return DomainResult.Success();
        }
    }
}