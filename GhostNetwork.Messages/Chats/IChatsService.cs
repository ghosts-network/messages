using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.Validation;

namespace GhostNetwork.Messages.Chats;

public interface IChatsService
{
    Task<(IEnumerable<Chat>, long)> SearchAsync(int skip, int take, Guid userId);

    Task<Chat> GetByIdAsync(Guid id);

    Task<(DomainResult, Chat)> CreateAsync(string name, IReadOnlyCollection<UserInfo> participants);

    Task<DomainResult> UpdateAsync(Chat chat);

    Task ReorderAsync(Guid id);

    Task DeleteAsync(Guid id);
}

public class ChatsService : IChatsService
{
    private readonly IChatsStorage chatStorage;
    private readonly IValidator<Chat> validator;

    public ChatsService(IChatsStorage chatStorage, IValidator<Chat> validator)
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

    public async Task<(DomainResult, Chat)> CreateAsync(string name, IReadOnlyCollection<UserInfo> participants)
    {
        var newChat = Chat.NewChat(name, participants);

        var result = await validator.ValidateAsync(newChat);
        if (!result.Successed)
        {
            return (result, default);
        }

        var chat = await chatStorage.CreateAsync(newChat);

        return (DomainResult.Success(), chat);
    }

    public async Task<DomainResult> UpdateAsync(Chat chat)
    {
        var result = await validator.ValidateAsync(chat);
        if (!result.Successed)
        {
            return result;
        }

        await chatStorage.UpdateAsync(chat);

        return DomainResult.Success();
    }

    public Task ReorderAsync(Guid id)
    {
        return chatStorage.ReorderAsync(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        await chatStorage.DeleteAsync(id);
    }
}