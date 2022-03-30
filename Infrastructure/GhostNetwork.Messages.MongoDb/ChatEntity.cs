using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Messages.MongoDb
{
    public class ChatEntity
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("senderId")]
        public Guid SenderId { get; set; }

        [BsonElement("receiverId")]
        public Guid ReceiverId { get; set; }
    }
}