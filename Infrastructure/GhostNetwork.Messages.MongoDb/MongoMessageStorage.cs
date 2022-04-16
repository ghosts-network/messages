using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Messages.Messages;
using MongoDB.Driver;

namespace GhostNetwork.Messages.MongoDb;

public class MongoMessageStorage : IMessageStorage
{
    private readonly MongoDbContext _context;

    public MongoMessageStorage(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Message>, long)> SearchAsync(int skip, int take, Guid chatId)
    {
        var filter = Builders<MessageEntity>.Filter.Eq(p => p.ChatId, chatId);

        var totalCount = await _context.Message.Find(filter).CountDocumentsAsync();

        var history = await _context.Message
            .Find(filter)
            .Skip(skip)
            .Limit(take)
            .ToListAsync();

        return (history.Select(ToDomain), totalCount);
    }

    public async Task<Message> GetByIdAsync(Guid id)
    {
        var filter = Builders<MessageEntity>.Filter.Eq(p => p.ChatId, id);

        var entity = await _context.Message.Find(filter).FirstOrDefaultAsync();

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Message> SendAsync(Message message)
    {
        var entity = new MessageEntity()
        {
            ChatId = message.ChatId,
            SenderId = message.SenderId,
            SentOn = message.SentOn,
            Data = message.Data
        };

        await _context.Message.InsertOneAsync(entity);

        return ToDomain(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var filter = Builders<MessageEntity>.Filter.Eq(p => p.Id, id);

        await _context.Message.DeleteOneAsync(filter);
    }

    public async Task UpdateAsync(Guid id, string message)
    {
        var filter = Builders<MessageEntity>.Filter.Eq(p => p.Id, id);

        var update = Builders<MessageEntity>.Update
            .Set(p => p.Data, message)
            .Set(p => p.SentOn, DateTimeOffset.Now)
            .Set(p => p.IsUpdated, true);

        await _context.Message.UpdateOneAsync(filter, update);
    }

    private static Message ToDomain(MessageEntity entity)
    {
        return new Message(
            entity.Id,
            entity.ChatId,
            entity.SenderId,
            entity.SentOn,
            entity.IsUpdated,
            entity.Data);
    }
}