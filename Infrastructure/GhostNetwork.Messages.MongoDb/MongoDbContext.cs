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
        
        
    }
}