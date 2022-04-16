using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace GhostNetwork.Messages.Chats
{
    public interface IChatService
    {
        Task<(IEnumerable<Chat>, long)> SearchAsync(int skip, int take, Guid userId);

        Task<Chat> GetByIdAsync(Guid id);

        Task<(DomainResult, Guid)> CreateAsync(string name, List<Guid> users);

        Task<DomainResult> UpdateAsync(Guid id, string name, List<Guid> users);

        Task DeleteAsync(Guid id);
    }

    public class ChatService : IChatService
    {
        private readonly IChatStorage _chatStorage;
        private readonly IValidator<ChatContext> _validator;

        public ChatService(IChatStorage chatStorage, IValidator<ChatContext> validator)
        {
            _chatStorage = chatStorage;
            _validator = validator;
        }

        public async Task<(IEnumerable<Chat>, long)> SearchAsync(int skip, int take, Guid userId)
        {
            return await _chatStorage.SearchAsync(skip, take, userId);
        }

        public async Task<Chat> GetByIdAsync(Guid id)
        {
            return await _chatStorage.GetByIdAsync(id);
        }

        public async Task<(DomainResult, Guid)> CreateAsync(string name, List<Guid> users)
        {
            var result = _validator.Validate(new ChatContext(name, users));

            if (!result.Successed)
            {
                return (result, Guid.Empty);
            }

            var chat = Chat.NewChat(name, users);

            var id = await _chatStorage.CreatAsync(chat);

            return (result, id);
        }

        public async Task<DomainResult> UpdateAsync(Guid id, string name, List<Guid> users)
        {
            var result = _validator.Validate(new ChatContext(name, users));

            if (!result.Successed)
            {
                return result;
            }

            var chat = await _chatStorage.GetByIdAsync(id);
            chat.Update(name, users);

            await _chatStorage.UpdateAsync(chat);

            return DomainResult.Success();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _chatStorage.DeleteAsync(id);
        }
    }
}