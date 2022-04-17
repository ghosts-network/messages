using MongoDB.Driver;

namespace GhostNetwork.Messages.MongoDb
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase database;

        public MongoDbContext(IMongoDatabase database)
        {
            this.database = database;
        }

        public IMongoCollection<ChatEntity> Chat =>
            database.GetCollection<ChatEntity>("chat");

        public IMongoCollection<MessageEntity> Message =>
            database.GetCollection<MessageEntity>("message");
    }
}