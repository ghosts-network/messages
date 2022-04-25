using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace GhostNetwork.Messages.Messages;

public interface IMessagesService
{
    Task<(IEnumerable<Message>, long, string)> SearchAsync(string lastMessageId, int take, Guid chatId);

    Task<Message> GetByIdAsync(string id);

    Task<(DomainResult, string)> SendAsync(Guid chatId, UserInfo author, string data);

    Task DeleteAsync(string id);

    Task<DomainResult> UpdateAsync(string id, string data, Guid userId);
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

    public async Task<(DomainResult, string)> SendAsync(Guid chatId, UserInfo author, string data)
    {
        var participantsCheck = await messageStorage.ParticipantsCheckAsync(author.Id);

        if (!participantsCheck)
        {
            return (DomainResult.Error("You are not a member of this chat!"), default);
        }

        var newMessage = Message.NewMessage(chatId, author, data);

        var result = validator.Validate(new MessageContext(data));

        if (!result.Successed)
        {
            return (result, default);
        }

        var id = await messageStorage.SendAsync(newMessage);

        return (result, id);
    }

    public async Task DeleteAsync(string id)
    {
        await messageStorage.DeleteAsync(id);
    }

    public async Task<DomainResult> UpdateAsync(string id, string data, Guid userId)
    {
        var message = await messageStorage.GetByIdAsync(id);

        if (message.Author.Id != userId)
        {
            return DomainResult.Error("You are not the author of this message");
        }

        var result = validator.Validate(new MessageContext(data));

        if (!result.Successed)
        {
            return result;
        }

        await messageStorage.UpdateAsync(id, data);

        return result;
    }
}