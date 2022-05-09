using System.Collections.Generic;
using System.Linq;
using GhostNetwork.Messages.Chats;
using GhostNetwork.Messages.Users;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Messages.Integrations.Chats;

public class ChatEntity
{
    public ObjectId Id { get; set; }

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