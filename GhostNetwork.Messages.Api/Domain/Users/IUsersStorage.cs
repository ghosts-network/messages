using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Messages.Users;

public interface IUsersStorage
{
    Task<IReadOnlyCollection<UserInfo>> SearchAsync(List<Guid> ids);
}