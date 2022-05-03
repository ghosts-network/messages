using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.Validation;

namespace GhostNetwork.Messages.Chats
{
    public interface IChatsService
    {
        Task<(IEnumerable<Chat>, long)> SearchAsync(int skip, int take, Guid userId);

        Task<Chat> GetByIdAsync(Guid id);

        Task<(DomainResult, Chat)> CreateAsync(string name, List<UserInfo> participants);

        Task<DomainResult> UpdateAsync(Guid id, string name, List<UserInfo> participants);

        Task DeleteAsync(Guid id);
    }

    public class ChatsService : IChatsService
    {
        private readonly IChatsStorage chatStorage;
        private readonly IValidator<ChatContext> validator;

        public ChatsService(IChatsStorage chatStorage, IValidator<ChatContext> validator)
        {
            this.chatStorage = chatStorage;
            this.validator = validator;
        }

        public async Task<(IEnumerable<Chat>, long)> SearchAsync(int skip, int take, Guid userId)
        {
            return await chatStorage.SearchAsync(skip, take, userId);
        }

        public async Task<Chat> GetByIdAsync(Guid id)
        {
            return await chatStorage.GetByIdAsync(id);
        }

        public async Task<(DomainResult, Chat)> CreateAsync(string name, List<UserInfo> participants)
        {
            var result = await validator.ValidateAsync(new ChatContext(name, participants));

            if (!result.Successed)
            {
                return (result, default);
            }

            var newChat = Chat.NewChat(name, participants);

            var chat = await chatStorage.CreatAsync(newChat);

            return (result, chat);
        }

        public async Task<DomainResult> UpdateAsync(Guid id, string name, List<UserInfo> participants)
        {
            var result = await validator.ValidateAsync(new ChatContext(name, participants));

            if (!result.Successed)
            {
                return result;
            }

            var chat = await chatStorage.GetByIdAsync(id);

            if (chat is null)
            {
                return DomainResult.Error("Chat is not found");
            }

            chat.Update(name, participants);

            await chatStorage.UpdateAsync(chat);

            return DomainResult.Success();
        }

        public async Task DeleteAsync(Guid id)
        {
            await chatStorage.DeleteAsync(id);
        }
    }
}