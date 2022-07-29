using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Messages.Chats;
using MongoDB.Driver;

namespace GhostNetwork.Messages.Integrations.Chats;

public class MongoChatStorage : IChatsStorage
{
    private readonly MongoDbContext context;

    public MongoChatStorage(MongoDbContext context)
    {
        this.context = context;
    }

    public async Task<(IReadOnlyCollection<Chat>, long)> SearchAsync(Filter filter, Pagination pagination)
    {
        var f = Builders<ChatEntity>.Filter.Where(c => c.Participants.Any(x => x.Id == filter.UserId));
        var p = string.IsNullOrEmpty(pagination.Cursor)
            ? Builders<ChatEntity>.Filter.Empty
            : Builders<ChatEntity>.Filter.Lt(c => c.Id, pagination.Cursor);
        var s = Builders<ChatEntity>.Sort.Descending(c => c.Order);

        var totalCount = await context.Chats.CountDocumentsAsync(f);

        var chats = await context.Chats
            .Find(f & p)
            .Sort(s)
            .Limit(pagination.Limit)
            .ToListAsync();

        return (chats.Select(entity => (Chat)entity).ToList(), totalCount);
    }

    public async Task<Chat> GetByIdAsync(string id)
    {
        var filter = Builders<ChatEntity>.Filter.Eq(p => p.Id, id);

        var entity = await context.Chats.Find(filter).FirstOrDefaultAsync();

        return (Chat)entity;
    }

    public async Task<Chat> InsertAsync(Chat chat)
    {
        var entity = new ChatEntity
        {
            Id = chat.Id,
            Name = chat.Name,
            Order = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Participants = chat.Participants.Select(x => new UserInfoEntity
            {
                Id = x.Id,
                FullName = x.FullName,
                AvatarUrl = x.AvatarUrl
            }).ToList()
        };

        await context.Chats.InsertOneAsync(entity);

        return (Chat)entity;
    }

    public async Task UpdateAsync(Chat chat)
    {
        var filter = Builders<ChatEntity>.Filter.Eq(p => p.Id, chat.Id);

        var update = Builders<ChatEntity>.Update
            .Set(p => p.Name, chat.Name)
            .Set(p => p.Participants, chat.Participants.Select(x => new UserInfoEntity { Id = x.Id, FullName = x.FullName, AvatarUrl = x.AvatarUrl }).ToList());

        await context.Chats.UpdateOneAsync(filter, update);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var filter = Builders<ChatEntity>.Filter.Eq(p => p.Id, id);

        var result = await context.Chats.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }

    public async Task ReorderAsync(string id)
    {
        var filter = Builders<ChatEntity>.Filter
            .Eq(p => p.Id, id);

        var update = Builders<ChatEntity>.Update
            .Set(p => p.Order, DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        await context.Chats.UpdateOneAsync(filter, update);
    }
}