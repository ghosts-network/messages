using System.Threading.Tasks;

namespace GhostNetwork.Messages;

public interface IUserProvider
{
    Task<UserInfo> GetByIdAsync(string id);
}