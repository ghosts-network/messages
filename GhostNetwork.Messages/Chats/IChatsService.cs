using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.Validation;

namespace GhostNetwork.Messages.Chats;

public interface IChatsService
{
    Task<IEnumerable<Chat>> SearchAsync(ChatFilter filter, Pagination pagination);

    Task<Chat> GetByIdAsync(Id id);

    Task<(DomainResult, Chat)> CreateAsync(string name, IReadOnlyCollection<UserInfo> participants);

    Task<DomainResult> UpdateAsync(Chat chat);

    Task ReorderAsync(Id id);

    Task DeleteAsync(Id id);
}

public class ChatsService : IChatsService
{
    private readonly IChatsStorage chatStorage;
    private readonly IValidator<Chat> validator;
    private readonly IIdProvider idProvider;

    public ChatsService(IChatsStorage chatStorage, IValidator<Chat> validator, IIdProvider idProvider)
    {
        this.chatStorage = chatStorage;
        this.validator = validator;
        this.idProvider = idProvider;
    }

    public Task<IEnumerable<Chat>> SearchAsync(ChatFilter filter, Pagination pagination)
    {
        return chatStorage.SearchAsync(filter, pagination);
    }

    public Task<Chat> GetByIdAsync(Id id)
    {
        return chatStorage.GetByIdAsync(id);
    }

    public async Task<(DomainResult, Chat)> CreateAsync(string name, IReadOnlyCollection<UserInfo> participants)
    {
        var newChat = Chat.NewChat(idProvider.Generate(), name, participants);

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

    public Task ReorderAsync(Id id)
    {
        return chatStorage.ReorderAsync(id);
    }

    public async Task DeleteAsync(Id id)
    {
        await chatStorage.DeleteAsync(id);
    }
}