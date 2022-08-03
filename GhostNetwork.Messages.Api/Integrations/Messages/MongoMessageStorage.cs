using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Domain;
using GhostNetwork.Messages.Api.Domain.Messages;
using GhostNetwork.Messages.Api.Domain.Users;
using GhostNetwork.Messages.Api.Integrations;
using GhostNetwork.Messages.Api.Integrations.Messages;
using MongoDB.Driver;

namespace GhostNetwork.Messages.Integrations.Messages;

public class MongoMessageStorage : IMessagesStorage
{
    private readonly MongoDbContext context;

    public MongoMessageStorage(MongoDbContext context)
    {
        this.context = context;
    }

    public async Task<(IReadOnlyCollection<Message>, long)> SearchAsync(Filter filter, Pagination pagination)
    {
        var f = Builders<MessageEntity>.Filter.Eq(m => m.ChatId, filter.ChatId);
        var p = string.IsNullOrEmpty(pagination.Cursor)
            ? Builders<MessageEntity>.Filter.Empty
            : Builders<MessageEntity>.Filter.Lt(c => c.Id, pagination.Cursor);
        var s = Builders<MessageEntity>.Sort.Descending(m => m.Id);

        var totalCount = await context.Messages.CountDocumentsAsync(f);

        var messages = await context.Messages
            .Find(f & p)
            .Sort(s)
            .Limit(pagination.Limit)
            .ToListAsync();

        return (messages.Select(ToDomain).ToList(), totalCount);
    }

    public async Task<Message> GetByIdAsync(string chatId, string id)
    {
        var filter = Builders<MessageEntity>.Filter.Eq(p => p.Id, id)
            & Builders<MessageEntity>.Filter.Eq(p => p.ChatId, chatId);

        var entity = await context.Messages.Find(filter).FirstOrDefaultAsync();

        return entity is null ? null : ToDomain(entity);
    }

    public async Task InsertAsync(Message message)
    {
        var entity = new MessageEntity
        {
            Id = message.Id,
            ChatId = message.ChatId,
            Author = (UserInfoEntity)message.Author,
            SentOn = message.SentOn,
            UpdatedOn = message.UpdatedOn,
            Content = message.Content
        };

        await context.Messages.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(Message message)
    {
        var filter = Builders<MessageEntity>.Filter.Eq(p => p.Id, message.Id);

        var update = Builders<MessageEntity>.Update
            .Set(p => p.UpdatedOn, message.UpdatedOn)
            .Set(p => p.Content, message.Content);

        await context.Messages.UpdateOneAsync(filter, update);
    }

    public async Task<bool> DeleteAsync(string chatId, string id)
    {
        var filter = Builders<MessageEntity>.Filter.Eq(p => p.Id, id)
            & Builders<MessageEntity>.Filter.Eq(p => p.ChatId, chatId);

        var result = await context.Messages.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }

    public Task DeleteByChatAsync(string chatId)
    {
        var filter = Builders<MessageEntity>.Filter.Eq(p => p.ChatId, chatId);

        return context.Messages.DeleteManyAsync(filter);
    }

    private static Message ToDomain(MessageEntity entity)
    {
        return new Message(
            entity.Id,
            entity.ChatId,
            (UserInfo)entity.Author,
            entity.SentOn,
            entity.UpdatedOn,
            entity.Content);
    }
}