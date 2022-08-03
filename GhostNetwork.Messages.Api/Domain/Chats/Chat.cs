using System.Collections.Generic;
using GhostNetwork.Messages.Api.Domain.Users;

namespace GhostNetwork.Messages.Api.Domain.Chats;

public record Chat(string Id, string Name, IReadOnlyCollection<UserInfo> Participants);
