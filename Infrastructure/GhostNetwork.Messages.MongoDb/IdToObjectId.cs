using MongoDB.Bson;

namespace GhostNetwork.Messages.MongoDb;

public static class IdToObjectId
{
    public static ObjectId ToObjectId(this Id id)
    {
        return ObjectId.TryParse(id.Value, out var oid)
            ? oid
            : ObjectId.Empty;
    }

    public static Id ToId(this ObjectId oid)
    {
        return new Id(oid.ToString());
    }
}