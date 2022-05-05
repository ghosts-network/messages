using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Messages.Chats;
using MongoDB.Driver;

namespace GhostNetwork.Messages.MongoDb;

public class MongoChatStorage : IChatsStorage
{
    private readonly MongoDbContext context;

    public MongoChatStorage(MongoDbContext context)
    {
        this.context = context;
    }

    public async Task<(IEnumerable<Chat>, long)> SearchAsync(int skip, int take, Guid userId)
    {
        var filter = Builders<ChatEntity>.Filter.Where(p => p.Participants.Any(x => x.Id == userId));
        var sort = Builders<ChatEntity>.Sort.Descending(p => p.Order);

        var totalCount = await context.Chat.CountDocumentsAsync(filter);

        var chats = await context.Chat
            .Find(filter)
            .Sort(sort)
            .Skip(skip)
            .Limit(take)
            .ToListAsync();

        return (chats.Select(entity => (Chat)entity).ToList(), totalCount);
    }

    public async Task<Chat> GetByIdAsync(Guid id)
    {
        var filter = Builders<ChatEntity>.Filter.Eq(p => p.Id, id);

        var entity = await context.Chat.Find(filter).FirstOrDefaultAsync();

        return (Chat)entity;
    }

    public async Task<Chat> CreateAsync(Chat chat)
    {
        var entity = new ChatEntity
        {
            Id = chat.Id,
            Name = chat.Name,
            Order = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Participants = chat.Participants.Select(x => new UserInfoEntity()
            {
                Id = x.Id,
                FullName = x.FullName,
                AvatarUrl = x.AvatarUrl
            }).ToList()
        };

        await context.Chat.InsertOneAsync(entity);

        return (Chat)entity;
    }

    public async Task UpdateAsync(Chat chat)
    {
        var filter = Builders<ChatEntity>.Filter.Eq(p => p.Id, chat.Id);

        var update = Builders<ChatEntity>.Update
            .Set(p => p.Name, chat.Name)
            .Set(p => p.Participants, chat.Participants.Select(x => new UserInfoEntity { Id = x.Id, FullName = x.FullName, AvatarUrl = x.AvatarUrl }).ToList());

        await context.Chat.UpdateOneAsync(filter, update);
    }

    public async Task DeleteAsync(Guid id)
    {
        var messageFilter = Builders<MessageEntity>.Filter.Eq(p => p.ChatId, id);
        var chatFilter = Builders<ChatEntity>.Filter.Eq(p => p.Id, id);

        await context.Message.DeleteManyAsync(messageFilter);
        await context.Chat.DeleteOneAsync(chatFilter);
    }

    public async Task ReorderAsync(Guid id)
    {
        var filter = Builders<ChatEntity>.Filter
            .Eq(p => p.Id, id);

        var update = Builders<ChatEntity>.Update
            .Set(p => p.Order, DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        await context.Chat.UpdateOneAsync(filter, update);
    }
}