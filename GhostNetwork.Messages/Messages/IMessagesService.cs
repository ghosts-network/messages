using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.Validation;
using GhostNetwork.Messages.Chats;

namespace GhostNetwork.Messages.Messages;

public interface IMessagesService
{
    Task<IEnumerable<Message>> SearchAsync(MessageFilter filter, Pagination pagination);

    Task<Message> GetByIdAsync(Id id);

    Task<(DomainResult, Id)> SendAsync(Id chatId, UserInfo author, string message);

    Task DeleteAsync(Id id);

    Task<DomainResult> UpdateAsync(Id messageId, string message, Guid userId);
}

public class MessagesService : IMessagesService
{
    private readonly IMessagesStorage messageStorage;
    private readonly IChatsService chatsService;
    private readonly IValidator<Message> validator;
    private readonly IIdProvider idProvider;

    public MessagesService(
        IMessagesStorage messageStorage,
        IValidator<Message> validator,
        IChatsService chatsService,
        IIdProvider idProvider)
    {
        this.messageStorage = messageStorage;
        this.chatsService = chatsService;
        this.validator = validator;
        this.idProvider = idProvider;
    }

    public Task<IEnumerable<Message>> SearchAsync(MessageFilter filter, Pagination pagination)
    {
        return messageStorage.SearchAsync(filter, pagination);
    }

    public Task<Message> GetByIdAsync(Id id)
    {
        return messageStorage.GetByIdAsync(id);
    }

    public async Task<(DomainResult, Id)> SendAsync(Id chatId, UserInfo author, string content)
    {
        var chat = await chatsService.GetByIdAsync(chatId);
        if (chat is null)
        {
            return (DomainResult.Error("Chat is not found"), default);
        }

        var message = Message.NewMessage(idProvider.Generate(), chatId, author, content);

        var result = await validator.ValidateAsync(message);
        if (!result.Successed)
        {
            return (result, null);
        }

        await messageStorage.SendAsync(message);

        return (result, message.Id);
    }

    public async Task<DomainResult> UpdateAsync(Id id, string content, Guid userId)
    {
        var message = await messageStorage.GetByIdAsync(id);
        if (message.Author.Id != userId)
        {
            return DomainResult.Error("You are not the author of this message");
        }

        message.Update(content);

        var result = await validator.ValidateAsync(message);
        if (!result.Successed)
        {
            return result;
        }

        await messageStorage.UpdateAsync(message);

        return DomainResult.Success();
    }

    public async Task DeleteAsync(Id id)
    {
        await messageStorage.DeleteAsync(id);
    }
}