using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GhostNetwork.Messages.MongoDb
{
    public class MongoChatStorage : IChatService
    {
        private readonly MongoDbContext _context;

        public MongoChatStorage(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Guid>, long)> SearchExistChatsAsync(int slip, int take, Guid userId)
        {
            var filter = Builders<ChatEntity>.Filter.Eq(p => p.ReceiverId, userId)
                         | Builders<ChatEntity>.Filter.Eq(p => p.SenderId, userId);

            var totalCount = await _context.Chat.Find(filter).CountDocumentsAsync();

            var existChats = await _context.Chat.Find(filter).ToListAsync();

            return (existChats.Select(x => x.Id), totalCount);
        }

        public async Task<Guid> GetExistChatByIdAsync(Guid chatId)
        {
            var filter = Builders<ChatEntity>.Filter.Eq(p => p.Id, chatId);

            var entity = await _context.Chat.Find(filter).FirstOrDefaultAsync();

            return entity?.Id ?? Guid.Empty;
        }

        public async Task<Guid> CreateNewChatAsync(Chat newChat)
        {
            var entity = new ChatEntity()
            {
                Id = newChat.Id,
                SenderId = newChat.SenderId,
                ReceiverId = newChat.ReceiverId
            };

            await _context.Chat.InsertOneAsync(entity);

            return entity.Id;
        }

        public async Task DeleteChatAsync(Guid chatId)
        {
            var filter = Builders<ChatEntity>.Filter.Eq(p => p.Id, chatId);

            await _context.Chat.DeleteManyAsync(filter);
        }

        public Task<IEnumerable<Message>> GetChatHistoryAsync(Guid chatId)
        {
            throw new NotImplementedException();
        }

        public async Task SendMessageAsync(Message message)
        {
            var entity = new MessageEntity()
            {
                ChatId = message.ChatId,
                SenderId = message.SenderId,
                SentOn = message.SentOn,
                Data = message.Data
            };

            await _context.Message.InsertOneAsync(entity);
        }

        public Task DeleteMessageAsync(string messageId)
        {
            if (!ObjectId.TryParse(messageId, out var oId))
            {
                return null;
            }

            throw new NotImplementedException();
        }

        public Task UpdateMessageAsync(string messageId, string message)
        {
            if (!ObjectId.TryParse(messageId, out var oId))
            {
                return null;
            }

            throw new NotImplementedException();
        }

        private static Message ToDomain(MessageEntity entity)
        {
            return new Message(
                entity.Id.ToString(),
                entity.ChatId,
                entity.SenderId,
                entity.SentOn,
                entity.Data);
        }
    }
}