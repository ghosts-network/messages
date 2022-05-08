using MongoDB.Bson;

namespace GhostNetwork.Messages.MongoDb;

public class ObjectIdProvider : IIdProvider
{
    public Id Generate()
    {
        return new Id(ObjectId.GenerateNewId().ToString());
    }
}
