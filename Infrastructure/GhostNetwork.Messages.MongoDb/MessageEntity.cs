using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Messages.MongoDb;

public class MessageEntity
{
    [BsonId]
    public Guid Id { get; set; }

    [BsonElement("chatId")]
    public Guid ChatId { get; set; }

    [BsonElement("senderId")]
    public Guid SenderId { get; set; }

    [BsonElement("sentOn")]
    public DateTimeOffset SentOn { get; set; }

    [BsonElement("data")]
    public string Data { get; set; }
}