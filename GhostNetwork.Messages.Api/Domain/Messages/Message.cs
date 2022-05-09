using System;
using GhostNetwork.Messages.Users;
using MongoDB.Bson;

namespace GhostNetwork.Messages.Api.Domain;

public record Message(
    ObjectId Id,
    ObjectId ChatId,
    UserInfo Author,
    DateTimeOffset SentOn,
    DateTimeOffset UpdatedOn,
    string Content);
