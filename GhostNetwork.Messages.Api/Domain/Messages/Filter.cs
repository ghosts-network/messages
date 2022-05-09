using MongoDB.Bson;

namespace GhostNetwork.Messages.Api.Domain;

public record Filter(ObjectId? ChatId);
