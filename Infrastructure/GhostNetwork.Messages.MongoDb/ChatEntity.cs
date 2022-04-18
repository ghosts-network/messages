﻿using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Messages.MongoDb
{
    public class ChatEntity
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("users")]
        public List<Guid> Users { get; set; }
    }
}