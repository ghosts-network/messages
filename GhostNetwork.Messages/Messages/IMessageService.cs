using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Messages.Messages;

namespace GhostNetwork.Messages;

public interface IMessageService
{
    Task<(IEnumerable<Message>, long)> SearchAsync(int skip, int take, Guid chatId);

    Task<(DomainResult, Message)> SendAsync(Guid chatId, Guid senderId, string data);

    Task DeleteAsync(Guid id);

    Task<DomainResult> UpdateAsync(Guid id, string data);
}

public class MessageService : IMessageService
{
    private readonly IMessageStorage _messageStorage;
    private readonly IValidator<MessageContext> _validator;

    public MessageService(IMessageStorage messageStorage, IValidator<MessageContext> validator)
    {
        _messageStorage = messageStorage;
        _validator = validator;
    }

    public async Task<(IEnumerable<Message>, long)> SearchAsync(int skip, int take, Guid chatId)
    {
        return await _messageStorage.SearchAsync(skip, take, chatId);
    }

    public async Task<(DomainResult, Message)> SendAsync(Guid chatId, Guid senderId, string data)
    {
        var newMessage = Message.NewMessage(chatId, senderId, data);
        var result = _validator.Validate(new MessageContext(data));

        if (!result.Successed)
        {
            return (result, default);
        }

        var message = await _messageStorage.SendAsync(newMessage);

        return (result, message);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _messageStorage.DeleteAsync(id);
    }

    public async Task<DomainResult> UpdateAsync(Guid id, string data)
    {
        var result = _validator.Validate(new MessageContext(data));

        if (!result.Successed)
        {
            return result;
        }

        await _messageStorage.UpdateAsync(id, data);

        return result;
    }
}