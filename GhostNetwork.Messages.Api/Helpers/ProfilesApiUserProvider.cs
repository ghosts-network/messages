using System;
using System.Collections;
using System.Collections.Generic;
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

    public async Task<UserInfo> GetByIdAsync(string id)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            return null;
        }

        try
        {
            var result = await profilesApi.GetByIdAsync(guid);
            return result == null
                ? null
                : new UserInfo(result.Id, $"{result.FirstName} {result.LastName}", result.ProfilePicture);
        }
        catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<UserInfo>> SearchAsync(List<string> ids)
    {
        var userIds = new List<Guid>();

        foreach (var id in ids)
        {
            if (!Guid.TryParse(id, out var guid))
            {
                return null;
            }

            userIds.Add(guid);
        }

        try
        {
            var result = await profilesApi.SearchByIdsAsync(new ProfilesQueryModel(userIds));

            if (result.Any())
            {
                return result.Select(x => new UserInfo(x.Id, $"{x.FirstName} {x.LastName}", x.ProfilePicture));
            }
        }
        catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
        {
            Console.WriteLine(ex);
            return null;
        }

        return null;
    }
}