using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Messages.Integrations.Messages;

public class MessageEntity
{
    public ObjectId Id { get; set; }

    [BsonElement("chatId")]
    public ObjectId ChatId { get; set; }

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