using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Messages.Api.Domain.Users;

public interface IUsersStorage
{
    Task<IReadOnlyCollection<UserInfo>> SearchAsync(List<Guid> ids);
}