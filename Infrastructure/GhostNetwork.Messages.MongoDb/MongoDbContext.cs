using MongoDB.Driver;

namespace GhostNetwork.Messages.MongoDb
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IMongoDatabase database)
        {
            _database = database;
        }

        public IMongoCollection<ChatEntity> Chat =>
            _database.GetCollection<ChatEntity>("chat");

        public IMongoCollection<MessageEntity> Message =>
            _database.GetCollection<MessageEntity>("message");
    }
}