using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Messages.MongoDb;

public class MessageEntity
{
    public ObjectId Id { get; set; }

    [BsonElement("chatId")]
    public ObjectId ChatId { get; set; }

    [BsonElement("author")]
    public UserInfoEntity Author { get; set; }

    [BsonElement("sentOn")]
    public DateTimeOffset SentOn { get; set; }

    [BsonElement("lastUpdateOn")]
    public DateTimeOffset LastUpdateOn { get; set; }

    [BsonElement("content")]
    public string Content { get; set; }
}