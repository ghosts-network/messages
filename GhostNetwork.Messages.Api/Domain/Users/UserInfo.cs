using System;

namespace GhostNetwork.Messages.Users;

public record UserInfo(Guid Id, string FullName, string AvatarUrl);
