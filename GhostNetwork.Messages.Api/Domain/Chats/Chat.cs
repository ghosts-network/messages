using System.Collections.Generic;
using GhostNetwork.Messages.Users;
using MongoDB.Bson;

namespace GhostNetwork.Messages.Chats;

public record Chat(ObjectId Id, string Name, IReadOnlyCollection<UserInfo> Participants);
