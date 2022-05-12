using System;
using GhostNetwork.Messages.Users;

namespace GhostNetwork.Messages.Domain;

public record Message(
    string Id,
    string ChatId,
    UserInfo Author,
    DateTimeOffset SentOn,
    DateTimeOffset UpdatedOn,
    string Content);
