using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Messages.Api.Integrations.Messages;

public class MessageEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("chatId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ChatId { get; set; }

    [BsonElement("author")]
    public UserInfoEntity Author { get; set; }

    [BsonElement("sentOn")]
    [BsonRepresentation(BsonType.String)]
    public DateTimeOffset SentOn { get; set; }

    [BsonElement("updatedOn")]
    [BsonRepresentation(BsonType.String)]
    public DateTimeOffset UpdatedOn { get; set; }

    [BsonElement("content")]
    public string Content { get; set; }
}