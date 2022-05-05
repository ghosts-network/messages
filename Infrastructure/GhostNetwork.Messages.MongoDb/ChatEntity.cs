using System;
using System.Collections.Generic;
using System.Linq;
using GhostNetwork.Messages.Chats;
using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Messages.MongoDb;

public class ChatEntity
{
    [BsonId]
    public Guid Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("participants")]
    public IList<UserInfoEntity> Participants { get; set; }

    [BsonElement("order")]
    public long Order { get; set; }

    public static explicit operator Chat(ChatEntity entity)
    {
        return entity == null
            ? null
            : new Chat(
                entity.Id,
                entity.Name,
                entity.Participants.Select(p => (UserInfo)p).ToList());
    }
}