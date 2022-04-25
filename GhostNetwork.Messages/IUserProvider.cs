using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Messages;

public interface IUserProvider
{
    Task<UserInfo> GetByIdAsync(string id);

    Task<IEnumerable<UserInfo>> SearchAsync(List<string> ids);
}