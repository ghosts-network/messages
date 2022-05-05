using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace GhostNetwork.Messages.MongoDb;

public class MongoDbContext
{
    private readonly IMongoDatabase database;

    static MongoDbContext()
    {
        var pack = new ConventionPack
        {
            new GuidAsStringRepresentationConvention()
        };

        ConventionRegistry.Register("GUIDs as strings Conventions", pack, _ => true);
    }

    public MongoDbContext(IMongoDatabase database)
    {
        this.database = database;
    }

    public IMongoCollection<ChatEntity> Chat =>
        database.GetCollection<ChatEntity>("chat");

    public IMongoCollection<MessageEntity> Message =>
        database.GetCollection<MessageEntity>("message");
}