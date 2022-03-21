using System;
using System.Threading.Tasks;

namespace GhostNetwork.Messages.MongoDb
{
    public class MongoChatStorage : IChatService
    {
        private readonly MongoDbContext _dbContext;

        public MongoChatStorage(MongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public Task SendMessage(Guid @from, Guid to, Message message)
        {
            throw new NotImplementedException();
        }

        public Task ConnectToChat(Guid chatId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMessage(Guid senderId, Guid chatId, Guid messageId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateMessage(Guid senderId, Guid chatId, Guid messageId, Message updateMessage)
        {
            throw new NotImplementedException();
        }
    }
}