using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace GhostNetwork.Messages.MongoDb
{
    public class MongoChatStorage : IChatStorage
    {
        private readonly MongoDbContext _context;

        public MongoChatStorage(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Guid>, long)> SearchChatsAsync(int skip, int take, Guid userId)
        {
            var filter = Builders<ChatEntity>.Filter.AnyEq(p => p.UsersIds, userId);

            var totalCount = await _context.Chat.Find(filter).CountDocumentsAsync();

            var existChats = await _context.Chat
                .Find(filter)
                .Skip(skip)
                .Limit(take)
                .ToListAsync();

            return (existChats.Select(x => x.Id), totalCount);
        }

        public async Task<Chat> GetChatByIdAsync(Guid chatId)
        {
            var filter = Builders<ChatEntity>.Filter.Eq(p => p.Id, chatId);

            var entity = await _context.Chat.Find(filter).FirstOrDefaultAsync();

            return entity is null ? null : ToDomain(entity);
        }

        public async Task<Guid> CreateNewChatAsync(Chat chat)
        {
            var entity = new ChatEntity()
            {
                Id = chat.Id,
                UsersIds = chat.UsersIds
            };

            await _context.Chat.InsertOneAsync(entity);

            return entity.Id;
        }

        public async Task AddNewUsersToChatAsync(Guid chatId, IEnumerable<Guid> users)
        {
            var filter = Builders<ChatEntity>.Filter.Eq(p => p.Id, chatId);

            var entity = await _context.Chat.Find(filter).FirstOrDefaultAsync();

            var update = Builders<ChatEntity>.Update
                .Set(p => p.UsersIds, users.Concat(entity.UsersIds));

            await _context.Chat.UpdateOneAsync(filter, update);
        }

        public async Task DeleteChatAsync(Guid chatId)
        {
            var messageFilter = Builders<MessageEntity>.Filter.Eq(p => p.ChatId, chatId);
            var chatFilter = Builders<ChatEntity>.Filter.Eq(p => p.Id, chatId);

            await _context.Message.DeleteManyAsync(messageFilter);
            await _context.Chat.DeleteOneAsync(chatFilter);
        }

        private static Chat ToDomain(ChatEntity entity)
        {
            return new Chat(
                entity.Id,
                entity.UsersIds);
        }
    }
}