using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Messages.MongoDb
{
    public class ChatEntity
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("usersIds")]
        public IEnumerable<Guid> UsersIds { get; set; }
    }
}