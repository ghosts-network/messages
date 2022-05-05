using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Profiles.Api;
using GhostNetwork.Profiles.Client;
using GhostNetwork.Profiles.Model;

namespace GhostNetwork.Messages.Api.Helpers;

public class ProfilesApiUserProvider : IUserProvider
{
    private readonly IProfilesApi profilesApi;

    public ProfilesApiUserProvider(IProfilesApi profilesApi)
    {
        this.profilesApi = profilesApi;
    }

    public async Task<UserInfo> GetByIdAsync(Guid id)
    {
        try
        {
            var result = await profilesApi.GetByIdAsync(id);
            return result == null
                ? null
                : new UserInfo(result.Id, $"{result.FirstName} {result.LastName}", result.ProfilePicture);
        }
        catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
        {
            return null;
        }
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