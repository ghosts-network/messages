using System.Collections.Generic;
using GhostNetwork.Messages.Users;

namespace GhostNetwork.Messages.Chats;

public record Chat(string Id, string Name, IReadOnlyCollection<UserInfo> Participants);
