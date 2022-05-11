using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Domain;
using GhostNetwork.Messages.Users;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GhostNetwork.Messages.Integrations.Messages;

public class MongoMessageStorage : IMessagesStorage
{
    private readonly MongoDbContext context;

    public MongoMessageStorage(MongoDbContext context)
    {
        this.context = context;
    }

    public async Task<IReadOnlyCollection<Message>> SearchAsync(Filter filter, Pagination pagination)
    {
        if (!ObjectId.TryParse(pagination.Cursor, out var cursor))
        {
            cursor = ObjectId.Empty;
        }

        var f = Builders<MessageEntity>.Filter.Eq(m => m.ChatId, filter.ChatId);
        var p = cursor != ObjectId.Empty
            ? Builders<MessageEntity>.Filter.Lt(c => c.Id, cursor)
            : Builders<MessageEntity>.Filter.Empty;
        var s = Builders<MessageEntity>.Sort.Descending(m => m.Id);

        var messages = await context.Messages
            .Find(f & p)
            .Sort(s)
            .Limit(pagination.Limit)
            .ToListAsync();

        return messages.Select(ToDomain).ToList();
    }

    public async Task<Message> GetByIdAsync(ObjectId chatId, ObjectId id)
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

    public async Task<bool> DeleteAsync(ObjectId chatId, ObjectId id)
    {
        var filter = Builders<MessageEntity>.Filter.Eq(p => p.Id, id)
            & Builders<MessageEntity>.Filter.Eq(p => p.ChatId, chatId);

        var result = await context.Messages.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }

    public Task DeleteByChatAsync(ObjectId chatId)
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