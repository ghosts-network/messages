using GhostNetwork.Messages.Api.Integrations.Chats;
using GhostNetwork.Messages.Api.Integrations.Messages;
using MongoDB.Driver;

namespace GhostNetwork.Messages.Api.Integrations;

public class MongoDbContext
{
    private readonly IMongoDatabase database;

    public MongoDbContext(IMongoDatabase database)
    {
        this.database = database;
    }

    public IMongoCollection<ChatEntity> Chats => database.GetCollection<ChatEntity>("chats");

    public IMongoCollection<MessageEntity> Messages => database.GetCollection<MessageEntity>("messages");
}