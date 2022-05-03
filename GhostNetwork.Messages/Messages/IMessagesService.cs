using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Validation;
using GhostNetwork.Messages.Chats;

namespace GhostNetwork.Messages.Messages;

public interface IMessagesService
{
    Task<(IEnumerable<Message>, long, string)> SearchAsync(string lastMessageId, int take, Guid chatId);

    Task<Message> GetByIdAsync(string id);

    Task<(DomainResult, string)> SendAsync(Guid chatId, UserInfo author, string message);

    Task DeleteAsync(string id);

    Task<DomainResult> UpdateAsync(string messageId, Guid chatId, string message, Guid userId);
}

public class MessagesService : IMessagesService
{
    private readonly IMessagesStorage messageStorage;
    private readonly IChatsService chatsService;
    private readonly IValidator<MessageContext> validator;

    public MessagesService(IMessagesStorage messageStorage, IValidator<MessageContext> validator, IChatsService chatsService)
    {
        this.messageStorage = messageStorage;
        this.chatsService = chatsService;
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

    public async Task<(DomainResult, string)> SendAsync(Guid chatId, UserInfo author, string message)
    {
        var chat = await chatsService.GetByIdAsync(chatId);

        if (chat is null)
        {
            return (DomainResult.Error("Chat is not found"), default);
        }

        var result = await validator.ValidateAsync(new MessageContext(message, author.Id, chat.Participants.Select(x => x.Id)));

        if (!result.Successed)
        {
            return (result, default);
        }

        var newMessage = Message.NewMessage(chatId, author, message);

        var id = await messageStorage.SendAsync(newMessage);

        return (result, id);
    }

    public async Task DeleteAsync(string id)
    {
        await messageStorage.DeleteAsync(id);
    }

    public async Task<DomainResult> UpdateAsync(string messageId, Guid chatId, string message, Guid userId)
    {
        var chat = await chatsService.GetByIdAsync(chatId);

        if (chat is null)
        {
            return DomainResult.Error("Chat is not found");
        }

        var result = await validator.ValidateAsync(new MessageContext(message, userId, chat.Participants.Select(x => x.Id)));

        if (!result.Successed)
        {
            return result;
        }

        var existMessage = await messageStorage.GetByIdAsync(messageId);

        if (existMessage.Author.Id != userId)
        {
            return DomainResult.Error("You are not the author of this message");
        }

        await messageStorage.UpdateAsync(messageId, message);

        return result;
    }
}