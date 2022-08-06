using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Domain.Users;
using GhostNetwork.Profiles.Api;
using GhostNetwork.Profiles.Model;

namespace GhostNetwork.Messages.Api.Integrations.Users;

public class RestUsersStorage : IUsersStorage
{
    private readonly IProfilesApi profilesApi;

    public RestUsersStorage(IProfilesApi profilesApi)
    {
        this.profilesApi = profilesApi;
    }

    public async Task<IReadOnlyCollection<UserInfo>> SearchAsync(List<Guid> ids)
    {
        if (!ids.Any())
        {
            return ImmutableArray<UserInfo>.Empty;
        }

        var profiles = await profilesApi.SearchByIdsAsync(new ProfilesQueryModel(ids));

        return profiles
            .Select(profile => new UserInfo(profile.Id, $"{profile.FirstName} {profile.LastName}", profile.ProfilePicture))
            .ToArray();
    }
}