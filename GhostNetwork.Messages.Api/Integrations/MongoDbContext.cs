using GhostNetwork.Messages.Integrations.Chats;
using GhostNetwork.Messages.Integrations.Messages;
using MongoDB.Driver;

namespace GhostNetwork.Messages.Integrations;

public class MongoDbContext
{
    private readonly IMongoDatabase database;

    public MongoDbContext(IMongoDatabase database)
    {
        this.database = database;
    }

    public IMongoCollection<ChatEntity> Chat => database.GetCollection<ChatEntity>("chats");

    public IMongoCollection<MessageEntity> Message => database.GetCollection<MessageEntity>("messages");
}