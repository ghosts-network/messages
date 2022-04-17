using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace GhostNetwork.Messages.Chats
{
    public interface IChatsService
    {
        Task<(IEnumerable<Chat>, long)> SearchAsync(int skip, int take, Guid userId);

        Task<Chat> GetByIdAsync(Guid id);

        Task<(DomainResult, Guid)> CreateAsync(string name, List<Guid> users);

        Task<DomainResult> UpdateAsync(Guid id, string name, List<Guid> users);

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

        public async Task<(DomainResult, Guid)> CreateAsync(string name, List<Guid> users)
        {
            var result = validator.Validate(new ChatContext(name, users));

            if (!result.Successed)
            {
                return (result, Guid.Empty);
            }

            var chat = Chat.NewChat(name, users);

            var id = await chatStorage.CreatAsync(chat);

            return (result, id);
        }

        public async Task<DomainResult> UpdateAsync(Guid id, string name, List<Guid> users)
        {
            var result = validator.Validate(new ChatContext(name, users));

            if (!result.Successed)
            {
                return result;
            }

            var chat = await chatStorage.GetByIdAsync(id);
            chat.Update(name, users);

            await chatStorage.UpdateAsync(chat);

            return DomainResult.Success();
        }

        public async Task DeleteAsync(Guid id)
        {
            await chatStorage.DeleteAsync(id);
        }
    }
}