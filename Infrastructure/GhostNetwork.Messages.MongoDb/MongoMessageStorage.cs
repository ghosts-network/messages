using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Messages.Messages;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GhostNetwork.Messages.MongoDb;

public class MongoMessageStorage : IMessagesStorage
{
    private readonly MongoDbContext context;

    public MongoMessageStorage(MongoDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<Message>> SearchAsync(MessageFilter filter, Pagination pagination)
    {
        if (!ObjectId.TryParse(pagination.Cursor, out var cursor))
        {
            cursor = ObjectId.Empty;
        }

        var f = Builders<MessageEntity>.Filter.Eq(m => m.ChatId, filter.ChatId.ToObjectId());
        var p = cursor != ObjectId.Empty
            ? Builders<MessageEntity>.Filter.Gt(c => c.Id, cursor)
            : Builders<MessageEntity>.Filter.Empty;
        var s = Builders<MessageEntity>.Sort.Descending(m => m.SentOn);

        var messages = await context.Message
            .Find(f & p)
            .Sort(s)
            .Limit(pagination.Limit)
            .ToListAsync();

        return messages.Select(ToDomain).ToList();
    }

    public async Task<Message> GetByIdAsync(Id id)
    {
        var filter = Builders<MessageEntity>.Filter.Eq(p => p.Id, id.ToObjectId());

        var entity = await context.Message.Find(filter).FirstOrDefaultAsync();

        return entity is null ? null : ToDomain(entity);
    }

    public async Task SendAsync(Message message)
    {
        var entity = new MessageEntity
        {
            Id = message.Id.ToObjectId(),
            ChatId = message.ChatId.ToObjectId(),
            Author = (UserInfoEntity)message.Author,
            SentOn = message.SentOn,
            Content = message.Content
        };

        await context.Message.InsertOneAsync(entity);
    }

    public async Task DeleteAsync(Id id)
    {
        var filter = Builders<MessageEntity>.Filter.Eq(p => p.Id, id.ToObjectId());

        await context.Message.DeleteOneAsync(filter);
    }

    public async Task UpdateAsync(Message message)
    {
        var filter = Builders<MessageEntity>.Filter.Eq(p => p.Id, message.Id.ToObjectId());

        var update = Builders<MessageEntity>.Update
            .Set(p => p.LastUpdateOn, message.UpdatedOn)
            .Set(p => p.Content, message.Content);

        await context.Message.UpdateOneAsync(filter, update);
    }

    private static Message ToDomain(MessageEntity entity)
    {
        return new Message(
            entity.Id.ToId(),
            entity.ChatId.ToId(),
            (UserInfo)entity.Author,
            entity.SentOn,
            entity.LastUpdateOn,
            entity.Content);
    }
}