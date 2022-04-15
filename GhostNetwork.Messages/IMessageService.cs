using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace GhostNetwork.Messages;

public interface IMessageService
{
    Task<(IEnumerable<Message>, long)> GetChatHistoryAsync(int skip, int take, Guid chatId);

    Task<(DomainResult, Message)> SendMessageAsync(Guid chatId, Guid senderId, string data);

    Task DeleteMessageAsync(Guid id);

    Task<DomainResult> UpdateMessageAsync(Guid id, string data);
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

    public async Task<(IEnumerable<Message>, long)> GetChatHistoryAsync(int skip, int take, Guid chatId)
    {
        return await _messageStorage.GetChatHistoryAsync(skip, take, chatId);
    }

    public async Task<(DomainResult, Message)> SendMessageAsync(Guid chatId, Guid senderId, string data)
    {
        var newMessage = Message.NewMessage(chatId, senderId, data);
        var result = _validator.Validate(new MessageContext(data));

        if (!result.Successed)
        {
            return (result, default);
        }

        var message = await _messageStorage.SendMessageAsync(newMessage);

        return (result, message);
    }

    public async Task DeleteMessageAsync(Guid id)
    {
        await _messageStorage.DeleteMessageAsync(id);
    }

    public async Task<DomainResult> UpdateMessageAsync(Guid id, string data)
    {
        var result = _validator.Validate(new MessageContext(data));

        if (!result.Successed)
        {
            return result;
        }

        await _messageStorage.UpdateMessageAsync(id, data);

        return result;
    }
}