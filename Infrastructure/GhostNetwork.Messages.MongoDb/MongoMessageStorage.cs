using System;
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

    public async Task<(IEnumerable<Message>, long, string)> SearchAsync(string lastMessageId, int take, Guid chatId)
    {
        var filter = Builders<MessageEntity>.Filter.Eq(p => p.ChatId, chatId);
        var sorting = Builders<MessageEntity>.Sort.Descending(p => p.SentOn);

        if (lastMessageId is not null)
        {
            filter &= Builders<MessageEntity>.Filter.Lt(x => x.Id, ObjectId.Parse(lastMessageId));
        }

        var totalCount = await context.Message.Find(filter).CountDocumentsAsync();

        var messages = await context.Message

            .Find(filter)
            .Sort(sorting)
            .Limit(take)
            .ToListAsync();

        var lastMessage = messages.Any() ? messages[^1].Id.ToString() : null;

        return (messages.Select(ToDomain), totalCount, lastMessage);
    }

    public async Task<Message> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var oId))
        {
            return null;
        }

        var filter = Builders<MessageEntity>.Filter.Eq(p => p.Id, oId);

        var entity = await context.Message.Find(filter).FirstOrDefaultAsync();

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<bool> ParticipantsCheckAsync(Guid userId)
    {
        var filter = Builders<ChatEntity>.Filter.Where(p => p.Participants.Any(x => x.Id == userId));

        var entity = await context.Chat.Find(filter).FirstOrDefaultAsync();

        return entity is not null;
    }

    public async Task<Message> SendAsync(Message message)
    {
        var entity = new MessageEntity()
        {
            ChatId = message.ChatId,
            Author = (UserInfoEntity)message.Author,
            SentOn = message.SentOn,
            Data = message.Data
        };

        await context.Message.InsertOneAsync(entity);

        return ToDomain(entity);
    }

    public async Task DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var oId))
        {
            return;
        }

        var filter = Builders<MessageEntity>.Filter.Eq(p => p.Id, oId);

        await context.Message.DeleteOneAsync(filter);
    }

    public async Task UpdateAsync(string id, string message)
    {
        if (!ObjectId.TryParse(id, out var oId))
        {
            return;
        }

        var filter = Builders<MessageEntity>.Filter.Eq(p => p.Id, oId);

        var update = Builders<MessageEntity>.Update
            .Set(p => p.SentOn, DateTimeOffset.Now)
            .Set(p => p.IsUpdated, true)
            .Set(p => p.Data, message);

        await context.Message.UpdateOneAsync(filter, update);
    }

    private static Message ToDomain(MessageEntity entity)
    {
        return new Message(
            entity.Id.ToString(),
            entity.ChatId,
            (UserInfo)entity.Author,
            entity.SentOn,
            entity.IsUpdated,
            entity.Data);
    }
}