using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Messages.Chats;
using MongoDB.Driver;

namespace GhostNetwork.Messages.MongoDb
{
    public class MongoChatStorage : IChatsStorage
    {
        private readonly MongoDbContext context;

        public MongoChatStorage(MongoDbContext context)
        {
            this.context = context;
        }

        public async Task<(IEnumerable<Chat>, long)> SearchAsync(int skip, int take, Guid userId)
        {
            var filter = Builders<ChatEntity>.Filter.AnyEq(p => p.Users, userId);

            var totalCount = await context.Chat.Find(filter).CountDocumentsAsync();

            var chats = await context.Chat
                .Find(filter)
                .Skip(skip)
                .Limit(take)
                .ToListAsync();

            return (chats.Select(ToDomain), totalCount);
        }

        public async Task<Chat> GetByIdAsync(Guid id)
        {
            var filter = Builders<ChatEntity>.Filter.Eq(p => p.Id, id);

            var entity = await context.Chat.Find(filter).FirstOrDefaultAsync();

            return entity is null ? null : ToDomain(entity);
        }

        public async Task<Guid> CreatAsync(Chat chat)
        {
            var entity = new ChatEntity()
            {
                Id = chat.Id,
                Name = chat.Name,
                Users = chat.Users
            };

            await context.Chat.InsertOneAsync(entity);

            return entity.Id;
        }

        public async Task UpdateAsync(Chat chat)
        {
            var filter = Builders<ChatEntity>.Filter.Eq(p => p.Id, chat.Id);

            var update = Builders<ChatEntity>.Update
                .Set(p => p.Users, chat.Users)
                .Set(p => p.Name, chat.Name);

            await context.Chat.UpdateOneAsync(filter, update);
        }

        public async Task DeleteAsync(Guid id)
        {
            var messageFilter = Builders<MessageEntity>.Filter.Eq(p => p.ChatId, id);
            var chatFilter = Builders<ChatEntity>.Filter.Eq(p => p.Id, id);

            await context.Message.DeleteManyAsync(messageFilter);
            await context.Chat.DeleteOneAsync(chatFilter);
        }

        private static Chat ToDomain(ChatEntity entity)
        {
            return new Chat(
                entity.Id,
                entity.Name,
                entity.Users);
        }
    }
}