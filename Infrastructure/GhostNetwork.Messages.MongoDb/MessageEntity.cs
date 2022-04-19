using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Messages.MongoDb;

public class MessageEntity
{
    public ObjectId Id { get; set; }

    [BsonElement("chatId")]
    public Guid ChatId { get; set; }

    [BsonElement("author")]
    public UserInfoEntity Author { get; set; }

    [BsonElement("sentOn")]
    public DateTimeOffset SentOn { get; set; }

    [BsonElement("isUpdated")]
    public bool IsUpdated { get; set; }

    [BsonElement("data")]
    public string Data { get; set; }
}