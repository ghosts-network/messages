﻿using System.Collections.Generic;
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

    public async Task<IEnumerable<Message>> SearchAsync(Filter filter, Pagination pagination)
    {
        if (!ObjectId.TryParse(pagination.Cursor, out var cursor))
        {
            cursor = ObjectId.Empty;
        }

        var f = Builders<MessageEntity>.Filter.Eq(m => m.ChatId, filter.ChatId);
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

    public async Task<Message> GetByIdAsync(ObjectId id)
    {
        var filter = Builders<MessageEntity>.Filter.Eq(p => p.Id, id);

        var entity = await context.Message.Find(filter).FirstOrDefaultAsync();

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
            Content = message.Content
        };

        await context.Message.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(Message message)
    {
        var filter = Builders<MessageEntity>.Filter.Eq(p => p.Id, message.Id);

        var update = Builders<MessageEntity>.Update
            .Set(p => p.LastUpdateOn, message.UpdatedOn)
            .Set(p => p.Content, message.Content);

        await context.Message.UpdateOneAsync(filter, update);
    }

    public async Task<bool> DeleteAsync(ObjectId id)
    {
        var filter = Builders<MessageEntity>.Filter.Eq(p => p.Id, id);

        var result = await context.Message.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }

    public Task DeleteByChatAsync(ObjectId chatId)
    {
        var filter = Builders<MessageEntity>.Filter.Eq(p => p.ChatId, chatId);

        return context.Message.DeleteManyAsync(filter);
    }

    private static Message ToDomain(MessageEntity entity)
    {
        return new Message(
            entity.Id,
            entity.ChatId,
            (UserInfo)entity.Author,
            entity.SentOn,
            entity.LastUpdateOn,
            entity.Content);
    }
}