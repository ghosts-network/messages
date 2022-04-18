using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace GhostNetwork.Messages.Messages;

public interface IMessagesService
{
    Task<(IEnumerable<Message>, long, string)> SearchAsync(string lastMessageId, int take, Guid chatId);

    Task<Message> GetByIdAsync(string id);

    Task<(DomainResult, Message)> SendAsync(Guid chatId, Guid senderId, string data);

    Task DeleteAsync(string id);

    Task<DomainResult> UpdateAsync(string id, string data);
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

    public async Task<(IEnumerable<Message>, long, string)> SearchAsync(string lastMessageId, int take, Guid chatId)
    {
        return await messageStorage.SearchAsync(lastMessageId, take, chatId);
    }

    public async Task<Message> GetByIdAsync(string id)
    {
        return await messageStorage.GetByIdAsync(id);
    }

    public async Task<(DomainResult, Message)> SendAsync(Guid chatId, Guid senderId, string data)
    {
        var participantsCheck = await messageStorage.ParticipantsCheckAsync(senderId);

        if (!participantsCheck)
        {
            return (DomainResult.Error("You are not a member of this chat!"), default);
        }

        var newMessage = Message.NewMessage(chatId, senderId, data);

        var result = validator.Validate(new MessageContext(data));

        if (!result.Successed)
        {
            return (result, default);
        }

        var message = await messageStorage.SendAsync(newMessage);

        return (result, message);
    }

    public async Task DeleteAsync(string id)
    {
        await messageStorage.DeleteAsync(id);
    }

    public async Task<DomainResult> UpdateAsync(string id, string data)
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