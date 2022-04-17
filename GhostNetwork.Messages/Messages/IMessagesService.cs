using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Messages.Messages;

namespace GhostNetwork.Messages;

public interface IMessagesService
{
    Task<(IEnumerable<Message>, long)> SearchAsync(int skip, int take, Guid chatId);

    Task<(DomainResult, Message)> SendAsync(Guid chatId, Guid senderId, string data);

    Task DeleteAsync(Guid id);

    Task<DomainResult> UpdateAsync(Guid id, string data);
}

public class MessagesService : IMessagesService
{
    private readonly IMessagesStorage messageStorage;
    private readonly IValidator<MessageContext> validator;

    public MessagesService(IMessagesStorage messageStorage, IValidator<MessageContext> validator)
    {
        this.messageStorage = messageStorage;
        this.validator = validator;
    }

    public async Task<(IEnumerable<Message>, long)> SearchAsync(int skip, int take, Guid chatId)
    {
        return await messageStorage.SearchAsync(skip, take, chatId);
    }

    public async Task<(DomainResult, Message)> SendAsync(Guid chatId, Guid senderId, string data)
    {
        var newMessage = Message.NewMessage(chatId, senderId, data);
        var result = validator.Validate(new MessageContext(data));

        if (!result.Successed)
        {
            return (result, default);
        }

        var message = await messageStorage.SendAsync(newMessage);

        return (result, message);
    }

    public async Task DeleteAsync(Guid id)
    {
        await messageStorage.DeleteAsync(id);
    }

    public async Task<DomainResult> UpdateAsync(Guid id, string data)
    {
        var result = validator.Validate(new MessageContext(data));

        if (!result.Successed)
        {
            return result;
        }

        await messageStorage.UpdateAsync(id, data);

        return result;
    }
}