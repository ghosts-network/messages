using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Messages;

public interface IUserProvider
{
    Task<UserInfo> GetByIdAsync(Guid id);

    Task<IReadOnlyCollection<UserInfo>> SearchAsync(List<Guid> ids);
}