using System;
using GhostNetwork.Messages.Api.Domain.Users;

namespace GhostNetwork.Messages.Api.Domain.Messages;

public record Message(
    string Id,
    string ChatId,
    UserInfo Author,
    DateTimeOffset SentOn,
    DateTimeOffset UpdatedOn,
    string Content);
