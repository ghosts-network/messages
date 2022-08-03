using System;

namespace GhostNetwork.Messages.Api.Domain.Users;

public record UserInfo(Guid Id, string FullName, string AvatarUrl);
